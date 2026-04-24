import os
import shutil

current_path = os.getcwd()

# Returns the folder the program is currently viewing
def get_current_path():
    return current_path

# Gets the full path of an item
def get_full_path(name):
    return os.path.join(current_path, name)

# Updates the current folder path
def set_current_path(path):
    global current_path
    current_path = path

# Gets all files and folders in the current directory
def list_files():
    return os.listdir(current_path)

# Creates a blank file in the current directory
def create_file(name):
    path = os.path.join(current_path, name)

    if os.path.exists(path):
        raise Exception("File already exists")

    with open(path, "w") as file:
        file.write("")

# Creates a new folder in the current directory
def create_folder(name):
    path = os.path.join(current_path, name)

    if os.path.exists(path):
        raise Exception("Folder already exists")

    os.mkdir(path)

# Deletes either a file or an empty folder
def delete_item(name):
    path = os.path.join(current_path, name)

    if not os.path.exists(path):
        raise Exception("File not found")

    if os.path.isdir(path):
        os.rmdir(path)
    else:
        os.remove(path)

# Reads the contents of a selected file
def read_file(name):
    path = os.path.join(current_path, name)

    if not os.path.exists(path):
        raise Exception("File not found")

    with open(path, "r") as file:
        return file.read()

# Saves new text into the selected file
def save_file(name, content):
    path = os.path.join(current_path, name)

    if not os.path.exists(path):
        raise Exception("File not found")

    with open(path, "w") as file:
        file.write(content)

# Checks whether the selected item is a folder
def is_folder(name):
    path = os.path.join(current_path, name)
    return os.path.isdir(path)

# Moves into the selected folder
def go_into_folder(name):
    path = os.path.join(current_path, name)

    if not os.path.isdir(path):
        raise Exception("Not a folder")

    set_current_path(path)

# Moves up one folder
def go_up():
    parent = os.path.dirname(current_path)
    set_current_path(parent)

# Renames file or folder
def rename_item(old_name, new_name):
    old_path = os.path.join(current_path, old_name)
    new_path = os.path.join(current_path, new_name)

    if os.path.exists(new_path):
        raise Exception("File with that name already exists")

    if not os.path.exists(old_path):
        raise Exception("File not found")

    os.rename(old_path, new_path)

# Copies a file or folder to the current directory
def copy_item(source_path, destination_folder):
    name = os.path.basename(source_path)
    new_path = os.path.join(destination_folder, name)

    if not os.path.exists(source_path):
        raise Exception("Original item not found")

    if os.path.exists(new_path):
        raise Exception("Item already exists here")

    if os.path.isdir(source_path):
        shutil.copytree(source_path, new_path)
    else:
        shutil.copy2(source_path, new_path)
