import argparse
import xmltodict
from xml.dom.minidom import parseString
import os

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
    with open(input_file, "r", encoding=encoding) as f:
        xml_string = f.read()

    data_dict = xmltodict.parse(xml_string)
    sub_item = "Language"

    if sub_item not in data_dict:
        if "Images" not in data_dict:
            print(f"Skipped file (no <Language> root): {input_file}")
            return
        else:
            sub_item = "Images"

    # Parent folder name of this XML file
    parent_folder = os.path.basename(os.path.dirname(input_file))
    output_dir = os.path.join(output_base, parent_folder)
    os.makedirs(output_dir, exist_ok=True)

    for lang_name, lang_content in data_dict[sub_item].items():
        if lang_name not in LANG_MAP:
            print(f"Skipped unmapped language: {lang_name} in {input_file}")
            continue

        lang_code = LANG_MAP[lang_name]

        # Build new XML structure
        new_root = {
            sub_item: {
                "@name": lang_name,
                **lang_content
            }
        }

        xml_str = xmltodict.unparse(new_root, pretty=True, encoding=encoding)
        xml_pretty = parseString(xml_str).toprettyxml(indent="  ")

        output_path = os.path.join(output_dir, f"{lang_code}.xml")
        with open(output_path, "w", encoding=encoding) as out_file:
            out_file.write(xml_pretty)

        print(f"Generated file: {output_path}")


def split_xml_in_directory(input_root, output_root, encoding="utf-8"):
    for root, _, files in os.walk(input_root):
        parts = os.path.normpath(root).split(os.sep)
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
