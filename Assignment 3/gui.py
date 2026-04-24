import tkinter as tk
from tkinter import messagebox, simpledialog
import file_ops

# Stores the full path of the copied item
clipboard_item = None

# Refreshes the listbox whenever the folder changes
def refresh_list():
    listbox.delete(0, tk.END)
    path_label.config(text=file_ops.get_current_path())

    try:
        for item in file_ops.list_files():
            listbox.insert(tk.END, item)
        status_label.config(text="Folder loaded")
    except:
        status_label.config(text="Error: Could not open folder")
        messagebox.showerror("Error", "Could not open folder")

# Creates a new file
def create_file():
    name = simpledialog.askstring("New File", "Enter file name:")

    if name:
        try:
            file_ops.create_file(name)
            refresh_list()
            status_label.config(text="File created")
        except:
            status_label.config(text="Error: Could not create file")
            messagebox.showerror("Error", "Could not create file")

# Creates a new folder
def create_folder():
    name = simpledialog.askstring("New Folder", "Enter folder name:")

    if name:
        try:
            file_ops.create_folder(name)
            refresh_list()
            status_label.config(text="Folder created")
        except:
            status_label.config(text="Error: Could not create folder")
            messagebox.showerror("Error", "Could not create folder")

# Deletes the selected file or folder
def delete_file():
    selected = listbox.get(tk.ACTIVE)

    if selected:
        confirm = messagebox.askyesno("Confirm Delete", "Are you sure you want to delete this?")

        if not confirm:
            status_label.config(text="Delete canceled")
            return

        try:
            file_ops.delete_item(selected)
            refresh_list()

            # Clear text editor after deleting a file
            if not file_ops.is_folder(selected):
                text.delete("1.0", tk.END)

            status_label.config(text="Item deleted")
        except:
            status_label.config(text="Error: Could not delete item")
            messagebox.showerror("Error", "Could not delete item")

# Opens a file or enters a folder
def open_file():
    selected = listbox.get(tk.ACTIVE)

    if selected:
        if file_ops.is_folder(selected):
            file_ops.go_into_folder(selected)
            refresh_list()
            status_label.config(text="Opened folder")
        else:
            try:
                content = file_ops.read_file(selected)
                text.delete("1.0", tk.END)
                text.insert(tk.END, content)
                status_label.config(text="File opened")
            except:
                status_label.config(text="Error: Could not open file")
                messagebox.showerror("Error", "Could not open file")

# Saves changes to the selected file
def save_file():
    selected = listbox.get(tk.ACTIVE)

    if selected:
        try:
            content = text.get("1.0", tk.END)
            file_ops.save_file(selected, content)
            status_label.config(text="File saved")
            messagebox.showinfo("Saved", "File saved")
        except:
            status_label.config(text="Error: Could not save file")
            messagebox.showerror("Error", "Could not save file")

# Moves up one folder
def go_up():
    file_ops.go_up()
    refresh_list()
    status_label.config(text="Moved up one folder")

# Renames the selected file or folder
def rename_file():
    selected = listbox.get(tk.ACTIVE)

    if selected:
        new_name = simpledialog.askstring("Rename", "Enter new name:")

        if new_name:
            try:
                file_ops.rename_item(selected, new_name)
                refresh_list()
                status_label.config(text="Item renamed")
            except:
                status_label.config(text="Error: Could not rename item")
                messagebox.showerror("Error", "Could not rename item")

# Stores selected item full path to clipboard
def copy_file():
    global clipboard_item

    selected = listbox.get(tk.ACTIVE)

    if selected:
        clipboard_item = file_ops.get_full_path(selected)
        status_label.config(text="Item copied")
    else:
        status_label.config(text="Nothing selected")

# Pastes copied item into current folder
def paste_file():
    global clipboard_item

    if clipboard_item:
        try:
            file_ops.copy_item(clipboard_item, file_ops.get_current_path())
            refresh_list()
            status_label.config(text="Item pasted")
        except Exception as e:
            status_label.config(text="Error: " + str(e))
            messagebox.showerror("Error", str(e))
    else:
        status_label.config(text="Nothing to paste")

# Builds and starts the Tkinter window
def run_app():
    global root, path_label, listbox, text
    global status_label

    root = tk.Tk()
    root.title("Simple File Manager")

    # Shows current path
    path_label = tk.Label(root, text=file_ops.get_current_path())
    path_label.pack()

    # Shows files and folders
    listbox = tk.Listbox(root, width=50)
    listbox.pack()

    # Text editor
    text = tk.Text(root, height=10)
    text.pack()

    # Button container
    button_frame = tk.Frame(root)
    button_frame.pack()

    # Buttons
    tk.Button(button_frame, text="Open", command=open_file).pack(side="left")
    tk.Button(button_frame, text="Save", command=save_file).pack(side="left")
    tk.Button(button_frame, text="New File", command=create_file).pack(side="left")
    tk.Button(button_frame, text="New Folder", command=create_folder).pack(side="left")
    tk.Button(button_frame, text="Delete", command=delete_file).pack(side="left")
    tk.Button(button_frame, text="Up", command=go_up).pack(side="left")
    tk.Button(button_frame, text="Rename", command=rename_file).pack(side="left")
    tk.Button(button_frame, text="Copy", command=copy_file).pack(side="left")
    tk.Button(button_frame, text="Paste", command=paste_file).pack(side="left")

    # Status label
    status_label = tk.Label(root, text="Ready")
    status_label.pack()

    refresh_list()
    root.mainloop()
