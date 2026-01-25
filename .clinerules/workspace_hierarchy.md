This workspace contains 2 repos :

- vop-pico : the app repos that contains the .NET MAUI app for the Pico (in vop-pico/VopPico.App)
- vop-core : the core repos that contains the core of VoP tools, like the React UI (in vop-core/ui)

When building :
1. build the vop-core
2. copy the vop-core/dist/ui content to vop-pico/VopPico.App/Resources/Raw/ui (clear this folder before copy)
3. build the vop-pico