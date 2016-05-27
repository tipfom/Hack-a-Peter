﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace hack_a_peter.Scenes.Strategy
{
    public class Button
    {
        public Button(UnitButton texture)
        {
            Texture = texture;
        }

        public delegate void OnClickEventHandler(object sender, EventArgs e);
        public event OnClickEventHandler OnClick;

        private Rectangle _Rectangle;
        public Rectangle Rectangle
        {
            get { return _Rectangle; }
            set { _Rectangle = value; }
        }

        private UnitButton _Texture;
        public UnitButton Texture
        {
            get { return _Texture; }
            set { _Texture = value; }
        }

        public bool IsInShape(int x, int y)
        {
            return (x > Rectangle.Left & x < Rectangle.Right & y > Rectangle.Top & y < Rectangle.Bottom);
        }

        public void TriggerOnClick()
        {
            if (OnClick != null)
            {
                OnClick(this, EventArgs.Empty);
            }
        }
    }
}
