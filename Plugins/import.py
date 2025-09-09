import argparse
import xml.etree.ElementTree as ET
from xml.dom.minidom import parseString
import os
import re

# Reverse mapping from language codes to language names
LANG_CODE_MAP = {
    "en-us": "English",
    "es-es": "Español",
    "fr-fr": "Français",
    "it-it": "Italiano",
    "zh-hans": "简体中文",
    "zh-hant": "繁體中文",
    "ru": "Русский",
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
            else:
                output_filename = f"{dir_name}Lang.xml"
            output_file = os.path.join(output_dir, output_filename)

            print(f"Processing directory: {root}")
            merge_xml_files_in_directory(root, output_file)

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Merge multiple language XML files into multilingual XML files.")
    parser.add_argument("input", help="Input root directory containing XXX/XXX/ subdirectories with language XML files")
    parser.add_argument("output", help="Output root directory")
    args = parser.parse_args()

    merge_xml_in_directory_tree(args.input, args.output)
