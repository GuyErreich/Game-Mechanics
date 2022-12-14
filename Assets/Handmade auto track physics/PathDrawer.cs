using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PathDrawer : MonoBehaviour
{
    [SerializeField] private List<Transform> points;
    [SerializeField, Range(1, 2)] private float force = 1f;
    [SerializeField, Range(0.01f, 1)] private float precision = 0.25f;

    private ArrowPath path;
    private LineRenderer line;
    private int lineSplits;

    private void Awake()
    {
        this.line = this.GetComponent<LineRenderer>();
        ResetPath();
    }

    private void ResetPath()
    {
        if(points == null || points.Count == 0)
            return;
        
        var direction = this.transform.forward;
        var velocity = (this.points[0].position - this.transform.position).magnitude * 0.5f;
        this.path = new ArrowPath(this.transform.position, this.points[0].position, direction, velocity);
        
        for (var i = 1; i < this.points.Count; i++) {
            this.path.AddSegment(this.points[i].position, this.force);
        }
    }

    private void LateUpdate() {
        this.ResetPath();

        if (this.path == null || path.NumOfSegments == 0) return;

        var positions = new List<Vector3>();

        var segmentIndex = 0;

        while(segmentIndex < this.path.NumOfSegments) {
            var t = 0f;

            while (t <= 1) {
                var segmentPoints = this.path.GetPointsInSegment(segmentIndex);
                positions.Add(BezireCurve.Cubic(segmentPoints[0], segmentPoints[1], segmentPoints[2], segmentPoints[3], t));

                t += this.precision;
            }

            segmentIndex++;
        }
        
        this.line.positionCount = positions.Count;
        this.line.SetPositions(positions.ToArray());
    }
}
