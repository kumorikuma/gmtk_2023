# First time setup

1. Pull repository
2. Setup [VSCode for Unity](https://code.visualstudio.com/docs/other/unity) and use **game.code-workspace** with VSCode for auto-format / auto-complete
2. Setup ReactUnity (see below)
3. Try running game and should be able to move around with WASD

# ReactUnity Setup

1. Install [node](https://nodejs.org/en/download) for windows
2. Open terminal and navigate to **/react**
3. Install node dependencies with **npm install**
4. Open Unity Project
5. Open **React/Quick Start** window inside Unity, and install dependencies
6. **When developing:** Run **npm start** inside **/react** folder
7. Test UI by going into play mode and on the **Services/ReactUnity** game object, change the **Debug Route** on the **UI Router** script.
8. **When building:** Note that **npm run build** needs to be run inside **/react** folder, which will generate a JS file inside **Assets/Resources/react** that the game will use in absence of a dev-server connection. **ReactBuildPreprocessor** should run this automatically, and the output can be viewed in the **React/Build Log** menu inside Unity.

Documentation: https://reactunity.github.io/  
Repo: https://github.com/ReactUnity/core  
Discord: https://discord.gg/UY2EFW5ZKG

# Helpful Links

Unity Input System: https://docs.unity3d.com/Packages/com.unity.inputsystem@1.5/manual/index.html 
