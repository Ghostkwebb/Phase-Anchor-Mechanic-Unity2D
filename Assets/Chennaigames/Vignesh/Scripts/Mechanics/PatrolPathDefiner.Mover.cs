using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PatrolPathDefiner
{
    /// <summary>
    /// The Mover class oscillates between start and end points of a path at a defined speed.
    /// </summary>
    public class Mover
    {
        PatrolPathDefiner path;
        float p = 0;
        float duration;
        float startTime;

        public Mover(PatrolPathDefiner path, float speed)
        {
            this.path = path;
            this.duration = (path.endPosition - path.startPosition).magnitude / speed;
            this.startTime = Time.time;
        }

        /// <summary>
        /// Get the position of the mover for the current frame.
        /// </summary>
        /// <value></value>
        public Vector2 Position
        {
            get
            {
                p = Mathf.InverseLerp(0, duration, Mathf.PingPong(Time.time - startTime, duration));
                return path.transform.TransformPoint(Vector2.Lerp(path.startPosition, path.endPosition, p));
            }
        }
    }
}
