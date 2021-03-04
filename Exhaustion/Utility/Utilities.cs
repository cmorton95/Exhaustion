using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Exhaustion.Utility
{
    public class Utilities
    {
        private static Sprite _sweat;
        private static Sprite _encumbered;

        public static Sprite SweatSprite
        {
            get
            {
                if (_sweat == null)
                {
                    //ugly horrible way of retrieving the encumbered sprite, if you know a better way *please* tell me
                    var wet = ObjectDB.instance.GetStatusEffect("Wet");
                    if (wet != null)
                    {
                        _sweat = wet.m_icon;
                    }
                }
                return _sweat;
            }
        }

        public static Sprite EncumberedSprite
        {
            get
            {
                if (_encumbered == null)
                {
                    var enc = ObjectDB.instance.GetStatusEffect("Encumbered");
                    if (enc != null)
                    {
                        _encumbered = enc.m_icon;
                    }
                }
                return _encumbered;
            }
        }
    }
}
