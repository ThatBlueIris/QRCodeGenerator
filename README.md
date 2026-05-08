# QR Code Generator

A minimalist QR Code generator for Windows. Built because online generators are usually cluttered, require accounts and not so offline friendly.

## 🚀 Key Features

- **Custom Colors:** Full Hex color support.
- **Logo Support:** Embed images into the QR center.
- **Batch Generate:** Generate multiple codes from a CSV/TXT list.
- **Transparency:** Toggle transparent backgrounds for PNG exports.
- **Status Feedback:** Toast notifications for save/copy/hub actions.
  
---

## 🛠️ Installation & Usage

### For Users (Fastest)
1. Go to the **[Releases](https://github.com/ThatBlueIris/QRCodeGenerator/releases)** tab.
2. Download the `QRCodeGenerator.zip`.
3. Extract and run `QRCodeGenerator.exe`.
   * *Note: Ensure `QRCoder.dll` stays in the same folder as the EXE.*

### For Developers (Build from Source)
1. **[Clone](https://youtu.be/dQw4w9WgXcQ?si=P2sv8Rep3iHB1zBB)** the repository.
2. Open `QRCodeGenerator.sln` in **Visual Studio**.
3. Restore NuGet packages.
4. Build in **Release** mode.

### YouTube Tutorial

QR Code cuz i wanna show off

<p align="center">
<img src="QR%20Code.png" width="300">
</p>

---

## 📂 Batch Generation Format
To use the batch generator, provide a `.csv` or `.txt` file formatted as follows:
```text
FileName1, [https://yourlink.com](https://yourlink.com)
FileName2, [https://anotherlink.com](https://anotherlink.com)
```
The engine will automatically sanitize filenames and generate high-resolution PNGs in your chosen folder.

---

## 🎨 Branding Note
The primary color was selected for its aesthetic and because I like blue.

## 🤝 Feedback & Suggestions
Suggestions or bugs can be logged in the **[Issues](https://github.com/ThatBlueIris/QRCodeGenerator/issues)** Portal.

## 💡Future Idea(s)
- SVG / Vector Export
- Local History Vault
- WiFi Access Point Creator

---

## 📄 License
This project is licensed under the [MIT License](LICENSE).
