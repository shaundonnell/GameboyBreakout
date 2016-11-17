using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace BreakingOut.GameObjects
{
    class ScreenShake
    {
        private float time;
        private float totalTime;
        private Vector2 position;
        private Random random;
        private float force;
        private bool shaking;

        public ScreenShake()
        {
            time = 0;
            random = new Random();
            shaking = false;
            totalTime = 0.3f;
        }

        public void Shake(float force)
        {
            Debug.WriteLine("calling shake");
            time = 0;
            position = new Vector2(0, 0);
            this.force = force;
            shaking = true;
        }

        public void Update()
        {
            if(!shaking)
                return;

            if (time < totalTime)
            {
                time += (1.0f / 60.0f);
                Debug.WriteLine("Shaking: " + time);
                float multiplier = 1.0f - (time / totalTime);
                float x = ((float)random.NextDouble() - 0.5f) * force * multiplier;
                float y = ((float)random.NextDouble() - 0.5f) * force * multiplier;
                position = new Vector2(x, y);
                Debug.WriteLine("position: " + position.ToString());
            }
            else
            {
                shaking = false;
                position = new Vector2(0, 0);
            }
        }

        public Vector2 GetPosition()
        {
            return position;
        }
    }
}
