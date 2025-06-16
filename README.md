# Nightreign Auto Storm Timer (or just "NAST")

A simple and lightweight timer designed to run alongside *Elden Ring: Nightreign*.  
It helps you track when storm phases begin and end, making it easier to stay focused and prepared during gameplay.

NAST uses OCR (Optical Character Recognition) to automatically detect the current phase of the expedition directly from the game screen, eliminating the need for manual input.

## 🔍 Recognized Expedition Phase Texts

The application detects the current expedition phase using OCR and supports the following text variations:

- **Portuguese**: `DIA I`, `DIA II`
- **English**: `DAY I`, `DAY II`
- **German**: `TAG I`, `TAG II`
- **Italian**: `GIORNO I`, `GIORNO II`
- **French**: `JOUR I`, `JOUR II`
 
  
## 📋 Features
- 🚀 Lightweight storm phase timer for *Elden Ring: Nightreign*
- ✨ OCR support integrated using Tesseract for text recognition from images/screenshots
- 💾 Config file to store display settings and FPS used for OCR capture
- 🛠️ Settings window to configure application preferences, including OCR capture FPS and other options
- 🎨 Improved and compact user interface designed to run alongside the game without distractions
- 🔊 Embedded `.wav` sound assets directly into the executable
- 🌍 Initial setup for multilingual text recognition support
- ⌨️ Timer control with comprehensive keyboard shortcuts.
- 💾 Config file for display and FPS to use in OCR capture
  
## 🕹️ Keyboard Shortcuts
| Command     | Description                              |
|-------------|------------------------------------------|
| Ctrl + Q    | Close the application                    |
| F1          | Start, pause, and advance the phase      |
| F2          | Reset the remaining time                 |
| F3          | Go to the previous phase                 |
| F4          | Go to the next phase                     |
| F5          | Toggle compact mode                      |
| F6          | Toggle automatic mode                    |
| F7          | Show/hide the help window                |
| F8          | Show/hide the configuration window       |

## 🚀 How to Use
- Extract the release contents anywhere you want.  
- Open the extracted folder and run `nast.exe` to start the application.

#### ⚠️ Important Note : If the map is open while the "DAY" message appears on screen, OCR detection will likely fail. In that case, you can manually trigger the timer cycle using F1.

## 📄 Licensing & Credits
- OCR powered by [Tesseract OCR](https://github.com/tesseract-ocr/tesseract) (Apache License 2.0)
