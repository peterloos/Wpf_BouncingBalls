using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BouncingBallsEx
{
    public class BouncingBall
    {
        public static readonly double Radius = 20;

        private Ellipse circle;   // display shape to display bouncing ball
        private Vector direction; // direction vector to move shape
        
        private double left; // according to 'Left' property of shape
        private double top;  // according to 'Top' property of shape

        public BouncingBall(Random rand)
        {
            // setup shape to display bouncing ball
            this.circle = new Ellipse();
            this.circle.Width = Radius * 2;
            this.circle.Height = Radius * 2;
            this.circle.Stroke = Brushes.Red;
            this.circle.StrokeThickness = 5;

            // initialize start position by random
            this.Left = Radius + 5 * Radius * rand.NextDouble();
            this.Top = Radius + 5 * Radius * rand.NextDouble();

            // setup direction vector
            this.direction = new Vector(rand.Next(-10, +10), rand.Next(-10, +10));
            this.direction.Normalize();
        }

        public double Left
        {
            get { return this.left; }
            set
            {
                this.left = value;
                this.circle.SetValue(Canvas.LeftProperty, this.left);
            }
        }

        public double Top
        {
            get { return this.top; }
            set
            {
                this.top = value;
                this.circle.SetValue(Canvas.TopProperty, this.top);
            }
        }

        public Vector Center
        {
            get
            {
                return new Vector (this.left + Radius, this.top + Radius);
            }
        }

        public Vector Direction
        {
            get { return this.direction; }
            set { this.direction = value; }
        }

        public Ellipse Circle
        {
            get { return this.circle; }
        }

        public void NormalizeDirection()
        {
            this.direction.Normalize();
        }
    }
}
