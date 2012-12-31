using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColonyCoalition.Collision
{
    class CollisionThread
    {
        //TODO:
        /*
         * make this in to a fully functional alternate thread where collisions are detected.
         * 
         * For the most part, this should be able to run independently of the main thread;
         * My thought is that the main thread will go through all of the rendered objects, and
         * as it updates the viewscreen also posts the new location of each object into the
         * collision thread. This way, the collision thread can simply loop independently of
         * the main thread and simply post back collision events whenever they occur. Some sort
         * of throttling will also need to be implemented so that it doesn't go crazy using up 
         * the entire core. (i.e. ## loops max per second?) Note that to make it as fast as 
         * possible, the collision tree will only accept spheres (a specified structure, with
         * radius,position, ID, and SUBID (with ID and SUBID) .. NOTE: Each ID is independant
         * and referes to a rendered object (which will have same ID). SUBID is for objects
         * with multiple spheres (such as the spaceship).
         * 
         * Because this game is effectively a 2d world, a moderately simple adaptation of the
         * quadtree structure could be used. (see wikipedia for quadtree structure). Basically,
         * each element has four sub-elements that refer to top-left, top-right, bottom-left, and
         * bottom-right.
         * The quadtree that I think will work will act as follows:
         *      QuadtreeNode:
         *          int topleft
         *          int bottomright
         *          // (quadtree nodes below will be initialized when needed!)
         *          topleft = QuadtreeNode
         *          topright = QuadtreeNode
         *          bottomleft = QuadtreeNode
         *          bottomright = QuadtreeNode
         *          itemlist = userboundingsphere[20]
         *          
         *      Notes:
         *      Each tree has a topleft and bottomright position.
         *      The maximum for each item will be a set value, i.e. 20. This will increase memory
         *      usage but limit allocation and deallocation.
         *      
         * 
         *      When inserting an item, the tree is checked recursively to see if the item
         *      fits entirely within any subnode, and if it does then recurses.
         *      When it gets to a node it is added to the list, or if the list is full is added to
         *      the parent. If all full, then item is pushed to independent list, to be inserted
         *      when it can be (and is treated as if it is part of top node until that time)
         *      
         *      When doing collision detection (normal run-through) - By the logic for which the tree
         *      is organized, an item is contained entirely within whichever node it occupies. Therefore,
         *      the only items it can collide with are those below it (and collision with those above
         *      it are assumed to already have been handled). However, the logic needs to be changed
         *      slightly because the items are likely always moving - therefore, before checking
         *      for collisions, the input list of movements will be checked (and if needed the item
         *      will be moved elsewhere in the tree).
         *      
         *      If a collision does occur, it is simply passed back to the main thread. (In the main,
         *      this list will be checked every cycle). (In the main thread, if a collision occured,
         *      the appropriate action will be found by tracing the two ID's to the objects in 
         *      question.
         *      
         *      One last thing - periodically the movement input list will be checked and if it grows
         *      too large then a 'force' will be put on the list - a traversal which doesn't detect
         *      collisions but simply reorders the tree to ensure optimal performance.
         *      
         * This quadtree may not be the most efficient usage of space and time for a small amount of
         * objects, but it is my hope that as the number of items grows it won't slow anything down
         * too much. Also, because it is run in a different thread from the main operation of the game,
         * it will hopefully never cause problems (other than perhaps a slight lag before collisions.).
         * 
         * One disadvanatage of this particular approach is that if an object is directly in the center 
         * of the screen, it will be collision-detected for every single other object. However, this is
         * still better than the O(N^2) average case where each object is simply checked against each 
         * other object. Also, because bigger objects will likely be broken down, there is less chance
         * that the user will position any particular bounding sphere directly in the middle (i.e.
         * the ship).
         * 
         * 
         */
    }
}
