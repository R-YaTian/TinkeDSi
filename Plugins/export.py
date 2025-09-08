import argparse
import xml.etree.ElementTree as ET
from xml.dom.minidom import parseString
import os
import re

LANG_MAP = {
    "English": "en-us",
    "Español": "es-es",
    "Français": "fr-fr",
    "Italiano": "it-it",
    "简体中文": "zh-hans",
    "繁體中文": "zh-hant",
    "Русский": "ru",
}

def process_xml_file(input_file, output_base, encoding="utf-8"):
    # Parse XML directly without using xmltodict to preserve mixed content order
    tree = ET.parse(input_file)
    root = tree.getroot()
    
    # Determine root element type
    if root.tag == "Language":
        sub_item = "Language"
        languages = {child.tag: child for child in root}
    elif root.tag == "Images":
        sub_item = "Images"
        languages = {child.tag: child for child in root}
    else:
        print(f"Skipped file (unsupported root): {input_file}")
        return

    # Get parent folder name
    parent_folder = os.path.basename(os.path.dirname(input_file))
    output_dir = os.path.join(output_base, parent_folder)
    os.makedirs(output_dir, exist_ok=True)

    for lang_name, lang_element in languages.items():
        if lang_name not in LANG_MAP:
            print(f"Skipped unmapped language: {lang_name} in {input_file}")
            continue

        lang_code = LANG_MAP[lang_name]

        # Create new root element
        new_root = ET.Element(sub_item)
        new_root.set("name", lang_name)
        
        # Copy all child elements while preserving original structure and content
        for child in lang_element:
            new_root.append(child)

        # Generate formatted XML with proper encoding
        xml_str = ET.tostring(new_root, encoding='unicode')
        
        # Use minidom for formatting with explicit encoding declaration
        dom = parseString(xml_str)
        xml_pretty = dom.toprettyxml(indent="  ", encoding=encoding)
        
        # Convert bytes to string if needed and remove extra blank lines
        if isinstance(xml_pretty, bytes):
            xml_pretty = xml_pretty.decode(encoding)
        
        xml_pretty = re.sub(r'\n\s*\n', '\n', xml_pretty)
        xml_pretty = '\n'.join(line for line in xml_pretty.split('\n') if line.strip())

        output_path = os.path.join(output_dir, f"{lang_code}.xml")
        with open(output_path, "w", encoding=encoding) as out_file:
            out_file.write(xml_pretty)

        print(f"Generated file: {output_path}")


def split_xml_in_directory(input_root, output_root, encoding="utf-8"):
    # Walk through directory tree to find XML files
    for root, _, files in os.walk(input_root):
        parts = os.path.normpath(root).split(os.sep)
        # Process files in directories where folder name matches parent folder name
        if len(parts) >= 2 and parts[-1] == parts[-2]:
            for file in files:
                if file.lower().endswith(".xml"):
                    input_file = os.path.join(root, file)
                    process_xml_file(input_file, output_root, encoding)


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Split multilingual XML files into separate language XML files.")
    parser.add_argument("input", help="Input root directory containing XML files")
    parser.add_argument("output", help="Output root directory")
    args = parser.parse_args()

    split_xml_in_directory(args.input, args.output)
