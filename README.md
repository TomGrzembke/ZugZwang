# ZugZwang
.. chess-like planning meets endless runner pace in a mobile setting. 
Figurines move automatically, swipe to manipulate the board.

- Created in Unity Engine.
- 2 Programmers.
- Self-set Task: Create a mobile game in Unity for Google Play Store in 10 weeks.

# How To Run
Head to Playstore: [here](https://play.google.com/store/apps/details?id=net.limitedheadspace.zugzwang)

WebGL embed at itch.io (Not the target Platform): [here](https://tom-grzembke.itch.io/zugzwang-pc-version)

## My Responsibility

System | Script | Purpose | 
  --- | --- | --- | 
  CoroutineOperation | [Read here](https://github.com/TomGrzembke/ZugZwang/blob/main/Assets/_Workdata/GameWorld/Environment/Scripts/CoroutineOperation.cs) | Acts as a Container for a [Coroutine](https://docs.unity3d.com/530/Documentation/ScriptReference/MonoBehaviour.StartCoroutine.html) to receive an event OnFinished and cancel it early with more status info.|
AutoMove | [Take a glance](https://github.com/TomGrzembke/ZugZwang/blob/main/Assets/_Workdata/GameWorld/Character/Scripts/AutoMove.cs) | Script Responsible for the Auto Movement of the Figurines.| 
ObjectPooling | [Flip Through](https://github.com/TomGrzembke/ZugZwang/blob/main/Assets/_Workdata/Utility/Scripts/ObjectPooling.cs) | General Object Pooling System for reusing VFX, Figurines, Environment sets and Playing field segments. I adapted and tailored it to our needs from this [Tutorial](https://youtu.be/Ah3epb2HGCw).| 

</div>

# Additional Info
This Project was displayed for 5 days at the Berlin IFA 2025.

- The primary VCS was peforce, that's why the initial commit contains most files.
- Release date: 15.08.2025
- Unity Version: 6000.0.51f1
- Team Size : 9

## Packages: 
- [Deadcows Mybox](https://github.com/Deadcows/MyBox.git) (Editor Utils and Handy Extension Methods)
- "New" Input System
- [ParticleEffectForUGUI](https://github.com/mob-sakai/ParticleEffectForUGUI)
