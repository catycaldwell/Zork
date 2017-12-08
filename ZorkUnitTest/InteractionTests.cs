﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;
using System.IO;
using Zork;
using Zork.Characters;
using Zork.Objects;

namespace ZorkUnitTest
{
    [TestClass]
    public class InteractionTests
    {
        [TestMethod]
        public void ChooseEnemyTest()
        {
            //TODO: ask how to test private methods and user input from console. (console.readlines)
            StringWriter writer = new StringWriter();
            Console.SetOut(writer);
            Point currentRoom = new Point(0, 0);
            Maze maze = new Maze(1, 1, currentRoom.X, currentRoom.Y);
            maze[currentRoom.X, currentRoom.Y].CharactersInRoom.Add(CharacterDefinitions.NPCS[0]);
            Interactions.ChooseEnemyMessage(maze, currentRoom);
            Assert.IsTrue(writer.ToString().Contains($"[{0 + 1}] {maze[currentRoom].CharactersInRoom[0].Name}"));
        }

        [TestMethod]
        public void FightStrongEnemyTest()
        {
            //TODO: ask how to test private methods
            Assert.IsFalse(Interactions.Fight(new NPC("sherrif_barney", 30, 100, new Weapon("Strong weapon", 10, "desc"), "desc"), new Player()));
        }

        [TestMethod]
        public void FightweakEnemyTest()
        {
            //TODO: ask how to test private methods
            Assert.IsTrue(Interactions.Fight(new NPC("sherrif_barney", 1, 10, null, "desc"), new Player()));
        }
    }
}
