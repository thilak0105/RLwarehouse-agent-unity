# Mark2

This repository contains the `Mark2` Unity project files.

Contents:
- Unity project source under `Assets/` and project settings
- Notebook and other files (if any)

Quick start — prepare and push to GitHub (terminal)

1. Change to the project folder (adjust path if different):

```zsh
cd "/Users/thilak/PythonFiles/Sem 7/Unity Projects/Mark2"
```

2. Initialize git, add files, and make the initial commit:

```zsh
# RLwarehouse-agent-unity (Mark2)

This repository contains the Unity project for the RL Warehouse agent (Mark2). It includes the Unity scene, scripts, assets and project settings required to open and run the project in the Unity Editor. Training artifacts and virtual environments are intentionally excluded from source control and are handled separately.

---

## Quick summary

- Unity project files: `Assets/`, `Packages/`, `ProjectSettings/` (already in the repo).
- Trained model for inference: `Assets/TrainedModels/WarehouseAgent.onnx`.
- Python training environment: `requirements.txt` (generated from the local `mlagents-env`).

This README explains how to open the project in Unity, run inference, recreate the Python training environment, and how we handle large files.

---

## Unity (open & run)

1. Install Unity Editor (recommended):

	- ProjectVersion: see `ProjectSettings/ProjectVersion.txt` — this project was created with: `6000.1.14f1`.
	- Use the same editor version or a compatible newer version. Unity will regenerate the `Library/` folder on first open (this can take a while).

2. Open the project:

	- Clone the repo and open the folder in Unity Hub or open the `Mark2` folder from the Unity Editor.

3. Scenes & play:

	- Open `Assets/Scenes/SampleScene.unity` (or the main scene your project uses) and press Play in the Editor.

4. Packages:

	- Unity Packages are defined in `Packages/manifest.json`. Unity will automatically download required packages (including `com.unity.ml-agents`) when the project loads.

---

## Model / Inference

- The agent model used for inference is checked in at: `Assets/TrainedModels/WarehouseAgent.onnx`.
- Unity (Barracuda) or ML-Agents inference components can load an ONNX file placed under `Assets/`.
- If you prefer `.nn` (Barracuda asset) you can convert or generate it and place it in the same folder.

If you need to replace the model, put the ONNX/NN file in `Assets/TrainedModels/` and restart the Editor (or reimport the asset).

---

## Training (Python / ML-Agents)

This repo does not include the full Python virtual environment. A `requirements.txt` has been added to help recreate the training environment.

To create a Python environment and install dependencies (example):

```zsh
# create and activate a venv (macOS / Linux)
python3 -m venv mlagents-env-py
source mlagents-env-py/bin/activate
pip install --upgrade pip
pip install -r requirements.txt
```

Common training command (example):

```zsh
# run training (adjust config and run id as needed)
mlagents-learn config/warehouse_config.yaml --run-id=Warehouse_Run001 --train
```

Notes:
- The `requirements.txt` was generated from a local `mlagents-env` in this workspace. That environment looked like a Windows virtualenv; package versions are captured, but adjust Python version/platform as needed.
- Training may require GPU-supporting PyTorch. Install the appropriate `torch` wheel for your CUDA / OS if needed.

---

## Large files & Git LFS

- We purposely exclude `Library/`, `mlagents-env/`, `results/` and other heavy/generated folders using `.gitignore` to keep the repo small.
- Only put models required for inference in `Assets/TrainedModels/` — small ONNX files are OK to commit directly.
- For very large files (checkpoints, huge ONNXs, model weights), use Git LFS. Example:

```zsh
brew install git-lfs        # macOS
git lfs install
git lfs track "*.pt" "*.onnx" "*.nn"
git add .gitattributes
```

---

## Project structure (important files)

- `Assets/` - Unity assets, scenes, scripts, prefabs, textures.
- `Packages/` - Unity package manifest (dependencies).
- `ProjectSettings/` - Unity project configuration (Editor, Graphics, Input, etc.).
- `Assets/TrainedModels/` - (added) ONNX/NN models used for inference in the Editor.
- `requirements.txt` - Python packages used by training (recreate venv with this file).
- `.gitignore` - rules to avoid committing generated/large files.

---

## Troubleshooting

- Unity won't open / shows package errors: open with the exact editor version in `ProjectSettings/ProjectVersion.txt` when possible.
- Missing packages: open Package Manager (Window → Package Manager) and press Restore/Resolve.
- Model not loading: confirm the model path (`Assets/TrainedModels/WarehouseAgent.onnx`) and that the component script expects that filename/path.
- Training errors (Python): ensure the Python venv has the correct `torch` for your platform and CUDA version.

---

## Contributing

- If you make changes to scenes or scripts, follow normal Git workflow: create a feature branch, commit changes, and open a PR.
- Avoid committing `Library/`, build artifacts, or virtualenv directories. Use `.gitignore` and Git LFS for large binary artifacts.

---

## License

Add a LICENSE file if you want to make the project open-source. If you want, I can add a permissive MIT license file for you.

---

If you'd like the README expanded with explicit editor screenshots, step-by-step training examples, or a dev helper script, tell me which parts to expand and I will add them.

4. If you need large checkpoints or many model files
	- Keep only the final ONNX(s) required for inference inside `Assets/` to keep the repo small. For large checkpoints (`*.pt`, very large `.onnx`) use Git LFS:

```zsh
brew install git-lfs
git lfs install
git lfs track "*.pt" "*.onnx"
git add .gitattributes
```

5. Quick checklist to open & run locally
	- Clone repo, open in Unity (matching version), confirm `Packages/manifest.json` installs dependencies, and run the scene `Assets/Scenes/SampleScene.unity` or the project scene specified in `Assets`.

If you'd like, I can add a short developer-runner script or expand the README with step-by-step instructions for training or inference. Tell me what you prefer.