import argparse
import xml.etree.ElementTree as ET
from xml.dom.minidom import parseString
import os
import re
import zipfile
import shutil

# Reverse mapping from language codes to language names
LANG_CODE_MAP = {
    "en-us": "English",
    "es-es": "Español",
    "fr-fr": "Français",
    "it-it": "Italiano",
    "zh-hans": "简体中文",
    # "zh-hant": "繁體中文",
    # "ru": "Русский",
}

def merge_xml_files_in_directory(input_dir, output_file):
    """
    Merge multiple language XML files in a directory into a single multilingual XML file.

    Args:
        input_dir: Directory containing language-specific XML files (e.g., en-us.xml, zh-hans.xml)
        output_file: Output XML file path (e.g., XXXLang.xml)
    """
    if not os.path.exists(input_dir):
        print(f"Input directory does not exist: {input_dir}")
        return

    # Find all XML files in the directory
    xml_files = [f for f in os.listdir(input_dir) if f.lower().endswith('.xml')]

    if not xml_files:
        print(f"No XML files found in directory: {input_dir}")
        return

    # Dictionary to store language data
    languages_data = {}
    root_element_type = None

    # Process each XML file
    for xml_file in xml_files:
        # Extract language code from filename (e.g., "en-us" from "en-us.xml")
        lang_code = os.path.splitext(xml_file)[0]

        if lang_code not in LANG_CODE_MAP:
            print(f"Skipped unknown language code: {lang_code} in file {xml_file}")
            continue

        lang_name = LANG_CODE_MAP[lang_code]
        file_path = os.path.join(input_dir, xml_file)

        try:
            # Parse the XML file
            tree = ET.parse(file_path)
            root = tree.getroot()

            # Determine root element type from first file
            if root_element_type is None:
                root_element_type = root.tag
            elif root_element_type != root.tag:
                print(f"Warning: Inconsistent root element type in {xml_file}. Expected {root_element_type}, got {root.tag}")

            # Store the language data (all child elements of the root)
            languages_data[lang_name] = list(root)

            print(f"Processed: {xml_file} -> {lang_name}")

        except ET.ParseError as e:
            print(f"Error parsing {xml_file}: {e}")
            continue

    if not languages_data:
        print("No valid language data found to merge")
        return

    # Create the merged XML structure
    merged_root = ET.Element(root_element_type)

    # Add each language as a child element
    for lang_name, child_elements in languages_data.items():
        lang_element = ET.SubElement(merged_root, lang_name)

        # Add all child elements to the language element
        for child in child_elements:
            lang_element.append(child)

    # Generate formatted XML
    xml_str = ET.tostring(merged_root, encoding='unicode')

    # Use minidom for formatting with proper encoding
    dom = parseString(xml_str)
    xml_pretty = dom.toprettyxml(indent="  ", encoding="utf-8")

    # Convert bytes to string if needed and clean up formatting
    if isinstance(xml_pretty, bytes):
        xml_pretty = xml_pretty.decode('utf-8')

    # Remove extra blank lines
    xml_pretty = re.sub(r'\n\s*\n', '\n', xml_pretty)
    xml_pretty = '\n'.join(line for line in xml_pretty.split('\n') if line.strip())

    # Write the merged XML file
    with open(output_file, 'w', encoding='utf-8') as f:
        f.write(xml_pretty)

    print(f"Merged XML file created: {output_file}")

def merge_xml_in_directory_tree(input_root, output_root, encoding="utf-8"):
    """
    Walk through directory tree and merge XML files in XXX/XXX/ subdirectories.

    Args:
        input_root: Root directory containing subdirectories with XML files
        output_root: Root directory for output files
    """
    for root, dirs, files in os.walk(input_root):
        # Check if this directory contains XML files
        xml_files = [f for f in files if f.lower().endswith('.xml')]

        if not xml_files:
            continue

        # Check if this matches the XXX/XXX/ pattern (two levels with same name)
        parts = os.path.normpath(root).split(os.sep)
        if len(parts) >= 2 and parts[-1] == parts[-2]:
            # This is a XXX/XXX/ directory, process it

            # Create output directory structure
            rel_path = os.path.relpath(root, input_root)
            output_dir = os.path.join(output_root, rel_path)
            os.makedirs(output_dir, exist_ok=True)

            # Generate output filename: use the directory name + "Lang.xml"
            dir_name = parts[-1]  # This is the XXX part
            if dir_name == "Images":
                output_filename = "Images.xml"
            elif dir_name == "Sounds":
                output_filename = "SoundLang.xml"
            elif dir_name == "Fonts":
                output_filename = "FontLang.xml"
            elif dir_name == "TETRIS DS":
                output_filename = "TETRISDSLang.xml"
            else:
                output_filename = f"{dir_name}Lang.xml"
            output_file = os.path.join(output_dir, output_filename)

            print(f"Processing directory: {root}")
            merge_xml_files_in_directory(root, output_file)

def extract_zip_to_temp(zip_path, temp_dir):
    """
    Extract zip file to temporary directory.

    Args:
        zip_path: Path to the zip file
        temp_dir: Temporary directory to extract to
    """
    print(f"Extracting {zip_path} to {temp_dir}")
    with zipfile.ZipFile(zip_path, 'r') as zip_ref:
        zip_ref.extractall(temp_dir)
    print(f"Extraction completed")

def copy_translated_files(translated_zip_path, temp_dir):
    """
    Extract translated zip and merge language files into the existing directory structure.

    Args:
        translated_zip_path: Path to the translated zip file
        temp_dir: Temporary directory where original files are extracted
    """
    # Create a temporary directory for translated files
    translated_temp = os.path.join(temp_dir, "translated_temp")
    os.makedirs(translated_temp, exist_ok=True)

    print(f"Extracting translated files from {translated_zip_path}")
    with zipfile.ZipFile(translated_zip_path, 'r') as zip_ref:
        zip_ref.extractall(translated_temp)

    # Get first-level directories in translated files (language folders)
    first_level_items = os.listdir(translated_temp)

    for lang_folder in first_level_items:
        lang_folder_path = os.path.join(translated_temp, lang_folder)
        if os.path.isdir(lang_folder_path):
            print(f"Processing language folder: {lang_folder}")

            # Walk through the language folder and copy all files to corresponding locations in temp_dir
            for root, dirs, files in os.walk(lang_folder_path):
                for file in files:
                    src_file = os.path.join(root, file)
                    # Get relative path from language folder
                    rel_path = os.path.relpath(src_file, lang_folder_path)
                    # Destination should be directly in temp_dir (not under language folder)
                    dest_file = os.path.join(temp_dir, rel_path)

                    # Create destination directory if it doesn't exist
                    os.makedirs(os.path.dirname(dest_file), exist_ok=True)

                    # Copy file (overwrite if exists)
                    shutil.copy2(src_file, dest_file)
                    print(f"Copied: {rel_path}")

    # Clean up translated temp directory
    shutil.rmtree(translated_temp)
    print("Translated files processing completed")

def process_zip_files(src_zip_path, translated_zip_path, output_root):
    """
    Process two zip files: extract source, copy translated files, then merge.

    Args:
        src_zip_path: Path to source zip file
        translated_zip_path: Path to translated zip file
        output_root: Output root directory
    """
    # Create temporary directory
    temp_dir = os.path.join(os.getcwd(), "tmp")

    # Clean up existing temp directory if it exists
    if os.path.exists(temp_dir):
        shutil.rmtree(temp_dir)

    os.makedirs(temp_dir, exist_ok=True)

    try:
        # Step 1: Extract source zip to temp directory
        extract_zip_to_temp(src_zip_path, temp_dir)

        # Step 2: Copy translated files to temp directory
        copy_translated_files(translated_zip_path, temp_dir)

        # Step 3: Merge XML files using Plugins directory as input
        plugins_dir = os.path.join(temp_dir, "Plugins")
        if os.path.exists(plugins_dir):
            print(f"Starting merge process with input: {plugins_dir}")
            merge_xml_in_directory_tree(plugins_dir, output_root)
        else:
            print(f"Warning: Plugins directory not found in {temp_dir}")
            print(f"Available directories: {os.listdir(temp_dir)}")

        # Step 4: Merge TinkeDSi main language files
        tinke_dir = os.path.join(temp_dir, "Tinke")
        if os.path.exists(tinke_dir):
            abs_output_root = os.path.abspath(output_root)
            parent_dir = os.path.dirname(abs_output_root)
            tinke_output_dir = os.path.join(parent_dir, "Tinke")
            shutil.copytree(tinke_dir, tinke_output_dir, dirs_exist_ok=True)
        else:
            print(f"Warning: TinkeDSi directory not found in {temp_dir}")

    finally:
        # Clean up temporary directory
        if os.path.exists(temp_dir):
            shutil.rmtree(temp_dir)
            print(f"Cleaned up temporary directory: {temp_dir}")

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Merge multiple language XML files from zip archives into multilingual XML files.")
    parser.add_argument("src_zip", help="Source zip file containing original files")
    parser.add_argument("translated_zip", help="Translated zip file containing translated language files")
    parser.add_argument("output", help="Output root directory")
    args = parser.parse_args()

    process_zip_files(args.src_zip, args.translated_zip, args.output)
