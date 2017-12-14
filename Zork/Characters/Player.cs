﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Zork.Objects;

namespace Zork.Characters
{
    public class Player : Character
    {
        #region properties
        public HashSet<string> Clues = new HashSet<string>();
        #endregion

        public delegate void MovedDelegate();
        public event MovedDelegate Moved;

        public Player(Room currentRoom) : base()
        {
            
            Name = "Sherlock Holmes";
            Description = "A very good investigator.";
            EquippedWeapon = null;
            Strength = 10;
            Health = MaxHealth;
            CurrentRoom = currentRoom;
        } 

        /// <summary>
        /// Prints what the player sees
        /// </summary>
        public void LookAround()
        {
            Console.WriteLine(CurrentRoom.DescribeRoom());
        }

        public void UseHealthPickup(HealthPickup h)
        {
            h.UseObject(this);
        }

        private string PrintHittingWithWeapon()
        {
            if (EquippedWeapon != null)
            {
                Console.Write($" with your mighty {EquippedWeapon.Name} ");
            }
            return "";
        }

        private string PrintGetHittedWithWeapon(NPC enemy)
        {
            if (enemy.EquippedWeapon != null)
            {
                Console.Write($" with his stupid {enemy.EquippedWeapon.Name} ");
            }
            return "";
        }

        protected override void FightOneRound(NPC enemy)
        {
            int enemyDamage = enemy.GenerateDamage();
            int myDamage = GenerateDamage();

            Console.WriteLine("You hit eachother...");
            enemy.TakeDamage(myDamage);
            TakeDamage(enemyDamage);

            Console.Write("You hit for: " + myDamage + PrintHittingWithWeapon());
            Console.Write($"\n{enemy.Name} hits you for:" + enemyDamage + PrintGetHittedWithWeapon(enemy));
            Console.WriteLine($"\nYou have {Health} hp left, he has {enemy.Health} hp left.");
        }

        internal void UseObject()
        {
            //check for useable items and display a list of which to use, otherwise show a non item available message
            List<BaseObject> useables = Inventory.Where(x => x is IUseableObject).ToList();
            if (useables.Count > 0)
            {
                Console.WriteLine("Choose which item you would like to use:");
                PrintUseableItems(useables);
            }
            else
            {

                Console.WriteLine("You have no useable items in your inventory.");
            }
        }

        private void PrintUseableItems(List<BaseObject> useableItems)
        {

            int counter = 0;
            foreach (BaseObject useable in useableItems)
            {
                counter++;
                if (useable is HealthPickup)
                {
                    Console.WriteLine($"[{counter}] : {useable.Name}, {useable.Description}, Heals for: {(useable as HealthPickup).Potency}");
                }
                else
                {
                    Console.WriteLine($"[{counter}] : {useable.Name}, {useable.Description}");
                }
            }
            ChooseObjectToUse(useableItems);
        }

        private void ChooseObjectToUse(List<BaseObject> useableItems)
        {
            string userInput = Console.ReadLine();

            if (userInput.Length <= 0)
            {
                return;
            }
            int userInputNumber;
            int.TryParse(userInput, out userInputNumber);
            userInputNumber--;
            if (userInputNumber >= 0 && userInputNumber < useableItems.Count)
            {
                (useableItems[userInputNumber] as IUseableObject).UseObject(this);
            }
        }

        public void Battle(Maze maze)
        {
            Console.WriteLine(CurrentRoom.DescribeCharactersInRoom());
            if(CurrentRoom.NPCsInRoom.Count == 0)
            {
                return;
            }
            int enemyNumber;
            if (int.TryParse(Console.ReadLine(), out enemyNumber) && enemyNumber > 0 && enemyNumber <= CurrentRoom.NPCsInRoom.Count)
            {
                NPC enemy = CurrentRoom.NPCsInRoom[enemyNumber - 1];
                Fight(enemy, maze);
            }
        }

        public void Fight(NPC enemy, Maze maze)
        {
            int turn = 0;
            while (enemy.Health > 0 && Health > 0)
            {                
                FightOneRound(enemy);
                ++turn;
                if (turn % enemy.LetsPlayerFleePerXRounds == 0 && AskFlee())
                {
                    Flee(maze);
                    return;
                }
            }
            CheckWhoWon(enemy);
            if (enemy.Health > 0)
            {
                enemy.KillThisNPC();
            }
        }

        protected bool AskFlee()
        {
            Console.WriteLine("Do you want to flee? Y/N");
            char userInputCharacter = (char)Console.Read();
            return (userInputCharacter == 'y' || userInputCharacter == 'Y');
        }

        /// <summary>
        /// Lets your character run to a random room
        /// </summary>
        /// <param name="allRooms"></param>
        public void Flee(Maze maze)
        {
            CurrentRoom = maze.GetRandomOtherRoom(CurrentRoom);
            Console.WriteLine("...What ...Where am i?");
        }
        
        #region talkMethods

        public void TryTalk()
        {
            Console.WriteLine("With who do you want to talk?");
            List<NPC> npcs =  CurrentRoom.NPCsInRoom;
            for (int i = 0; i < npcs.Count; i++)
            {
                Console.WriteLine($"[{i + 1}] {npcs[i].Name}");
            }
            int talkToNPCInt;
            int.TryParse(Console.ReadLine(), out talkToNPCInt);
            talkToNPCInt -= 1;
            if(talkToNPCInt >= 0 && talkToNPCInt <= npcs.Count)
            {
                npcs[talkToNPCInt].Talk(this);
            }
            else
            {
                Console.WriteLine("He's not here...");
                return;
            }
        }

        #endregion

        /// <summary>
        /// <summary>me="direction">Direction to go in</param>
        /// </summary>
        /// <param name="currentRoom">The room you're in</param>
        /// <param name="direction">The direction to go in</param>
        public void TryGo(Direction direction, Room[,] rooms)
        {
            Point towards = CurrentRoom.LocationOfRoom.Add(direction);
            if (towards.X < 0 || towards.X == Game.Width || towards.Y < 0 || towards.Y == Game.Height)
            {
                Console.WriteLine("You attempt to go " + direction.ToString().ToLower() + " but face the end of the world.");
            }
            else if (CurrentRoom.CanGoThere[direction])
            {
                CurrentRoom = rooms[towards.X, towards.Y];
                Moved?.Invoke();
            }
            else
            {
                Console.WriteLine("You cannot go " + direction.ToString().ToLower());
            }
        }
    }
}
