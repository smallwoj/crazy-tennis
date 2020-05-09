using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

interface Spectator
{
    void Cheer(float hype); // Reacts to the player scoring some points
    void setSortingOrder(int sortOrder);   // Sets the sprite's sorting order, so large crowds are drawn correctly
}