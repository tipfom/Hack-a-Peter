using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace hack_a_peter.Scenes.Strategy
{
    public enum UnitTexture
    {
        Bug1, Bug2, Hero1, Hero2
    }
    public class Unit
    {
        public Unit()
        {
            Weapons = new List<Weapon>();
        }

        private int _Health;
        public int Health
        {
            get { return _Health; }
            set { _Health = value; }
        }

        private int _MaxHealth;
        public int MaxHealth
        {
            get { return _MaxHealth; }
            set { _MaxHealth = value; }
        }


        private int _Armor;
        public int Armor
        {
            get { return _Armor; }
            set { _Armor = value; }
        }

        private int _MovementLeft;
        public int MovementLeft
        {
            get { return _MovementLeft; }
            set { _MovementLeft = value; }
        }

        private int _MoveSpeed;
        public int MoveSpeed
        {
            get { return _MoveSpeed; }
            set { _MoveSpeed = value; }
        }

        //0... Player
        //1... KI
        private int _Owner;
        public int Owner
        {
            get { return _Owner; }
            set { _Owner = value; }
        }

        private Point _Position;
        public Point Position

        {
            get { return _Position; }
            set { _Position = value; }
        }

        private UnitTexture _Texture;
        public UnitTexture Texture
        {
            get { return _Texture; }
            set { _Texture = value; }
        }

        private List<Weapon> _Weapons;
        public List<Weapon> Weapons
        {
            get { return _Weapons; }
            set { _Weapons = value; }
        }

        public Color GetColorByHealth()
        {
            //Current/Max = x/255
            int Value = Convert.ToInt32((float)Health / (float)MaxHealth * 255f);
            return new Color(255, Value, Value, 255);
        }
    }
}
