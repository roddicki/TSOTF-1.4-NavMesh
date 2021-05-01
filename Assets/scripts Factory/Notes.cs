// TO DO on MODEL
// add better lighting with spot lights
// make cubes aluminium (add softened reflection)

// TO SOLVE
//1. Add decals to Bays
//2. Magnet effect still not strong enough
//3. Implement a successfully picked up confirmation, if not reset()
//4. add contraints to the crane and gantry so if it hits edge goes back to centre




// Notes about colliders and triggers
// see https://www.youtube.com/watch?v=WFkbqdo2OI4 for Unity 3d: Collisions, Triggers, Rigidbodies, and Kinematics
////////////////////////////////////////////////////////////////////
// this is how I basically think about physics in Unity. A game object...
// - ...without collider: does not take part and/or care about physics; purely logic or visual
// - ...with trigger collider: a static (non-moving) detector area, for triggering logic when something enters it
// - ...with (non-trigger) collider, without rigidbody: static physical object, real, solid things that won't move, like walls, the ground, etc.
// - ...with (non-trigger) collider and non-kinematic rigidbody: dynamic physical object, a thing that moves according to the laws of physics, like boxes, balls, projectiles, etc.
// - ...with (non-trigger) collider and kinematic rigidbody: dynamic physical object, but it doesn't behave purely with physics, usually because it is moved by player input or animation or some other logic

//NOTES
// Magnet  hits ground is a collision // to implement this 
// This works by changing settings, V important!!!
// "Contacts Generation" setting in your Project Settings > Physics menu.
// You can set this to "Enable Kinematic Static Pairs" to allow kinematic rigidbodies to get callbacks on collisions 
// with static colliders.
// https://answers.unity.com/questions/209656/having-a-kinematic-rigidbody-detect-collision-with.html

