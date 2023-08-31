
import platform
import winreg
import os
import subprocess
import zipfile
import urllib.request

try:
    import vdf
except ImportError:
    import pip
    pip.__package__
    pip.main(['install', '--user', 'vdf'])
    import vdf

def extract_zip(zip_file_path, extract_to_directory):
    with zipfile.ZipFile(zip_file_path, 'r') as zip_ref:
        zip_ref.extractall(extract_to_directory)
    print(f"Extracted {zip_file_path} to {extract_to_directory}")

def delete_files(file_paths):
    for file_path in file_paths:
        if os.path.exists(file_path):
            os.remove(file_path)

def download_file_with_custom_headers(url, destination_path, headers=None):
    try:
        req = urllib.request.Request(url, headers=headers)
        with urllib.request.urlopen(req) as response, open(destination_path, 'wb') as out_file:
            data = response.read()
            out_file.write(data)
        print(f"Downloaded {url} to {destination_path}")
    except Exception as e:
        print(f"Error: {e}")

def download_file(url, destination_path):
    try:
        urllib.request.urlretrieve(url, destination_path)
        print(f"Downloaded {url} to {destination_path}")
    except Exception as e:
        print(f"Error: {e}")

def open_file_directory(directory_path):
    if os.path.exists(directory_path):
        if os.name == 'nt':  # Windows
            subprocess.Popen(['explorer', directory_path], shell=True)
    else:
        print("Directory does not exist.")

def print_files_in_directory(directory_path):
    if os.path.exists(directory_path) and os.path.isdir(directory_path):
        files = [file for file in os.listdir(directory_path) if os.path.isfile(os.path.join(directory_path, file))]
        for file in files:
            print(file)
    else:
        print("Directory does not exist.")


def get_steam_path(sub_key, value_name):
    try:
        key = winreg.OpenKey(winreg.HKEY_LOCAL_MACHINE, sub_key, 0, winreg.KEY_READ)
        value, value_type = winreg.QueryValueEx(key, value_name)
        winreg.CloseKey(key)
        return value
    except Exception as e:
        print(f"Error: {e}")
        return None
    
def main():
    if(platform.system() != "Windows"):
        print("This program only install the mod on Windows Computers.\n If you would Like to install this on another platform that can run Bonelab, you can attempt to follow the instructions in the project documentation as a guide to install it for your platform.")
        quit()
    arch = platform.architecture()[0]
    SteamInstallRegistrySubkey = r""
    if arch == "32bit":
        SteamInstallRegistrySubkey = r"SOFTWARE\Valve\Steam"
    elif arch == "64bit":
        SteamInstallRegistrySubkey = r"SOFTWARE\Wow6432Node\Valve\Steam"
    else:
        print("Could not determine Windows architecture.")

    SteamInstallDir = get_steam_path(SteamInstallRegistrySubkey, 'InstallPath')

    libraryFoldersFile = vdf.load(open(SteamInstallDir + r"\steamapps\libraryfolders.vdf"))
    bonelab_install_dir = r''
    
    for folder in libraryFoldersFile['libraryfolders']:
        for key in libraryFoldersFile['libraryfolders'][folder]['apps']:
            if key == '1592190':
                bonelab_install_dir = libraryFoldersFile['libraryfolders'][folder]['path'] + r"\steamapps\common\BONELAB"
        else:   
            continue
        break

    custom_headers = {
        "User-Agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3"
    }
    download_file_with_custom_headers("https://github.com/LavaGang/MelonLoader/releases/latest/download/MelonLoader.x64.zip", 'melonloader.zip', custom_headers)
    extract_zip('melonloader.zip', bonelab_install_dir)
    delete_files(['melonloader.zip', bonelab_install_dir+'\README.md', bonelab_install_dir+'\dobby.dll'])

    download_file_with_custom_headers("https://thunderstore.io/package/download/gnonme/BoneLib/2.2.1/", 'bonelib.zip',custom_headers)
    download_file_with_custom_headers("https://thunderstore.io/package/download/Maranara/Marrow_Cauldron/1.0.2/", 'marrow_cauldron.zip',custom_headers)
    
    extract_zip('bonelib.zip', bonelab_install_dir)
    extract_zip('marrow_cauldron.zip', bonelab_install_dir)
    delete_files(['bonelib.zip','marrow_cauldron.zip'])




if __name__ == "__main__":
    main()
