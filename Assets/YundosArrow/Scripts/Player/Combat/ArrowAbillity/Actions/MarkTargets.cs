using System;
using System.Collections.Generic;
using UnityEngine;
using YundosArrow.Scripts.Player.Combat.ArrowAbilities.Utils;

namespace YundosArrow.Scripts.Player.Combat.ArrowAbilities.Actions
{
    public static class MarkTargets
    {
        private static RaycastHit _hit;

        public static bool IsMarked { get => GlobalCollections.Targets != null && GlobalCollections.Targets.Count > 0f; }

        public static void Mark() {                
            Ray ray = Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f));

            if (Physics.SphereCast(ray, ArrowStats.AttackStats.MarkTargets.Radius, out _hit, ArrowStats.AttackStats.MarkTargets.Range, ArrowStats.AttackStats.MarkTargets.LayerMask)) {
                if (InputReceiver.Bool[InputReceiverType.ShootPressed]) {
                    if (!IsMarked) {
                        //Targets
                        GlobalCollections.Targets = new List<Transform>();
                        GlobalCollections.Targets.Add(_hit.transform);
                        
                        //Path
                        var segment = new Segment(ArrowStats.StartPoint.position, _hit.point);
                        GlobalCollections.Path = new LinearArrowPath(segment);
                        GlobalCollections.Path.ClosePath();
                    }
                    else {
                        if(GlobalCollections.Targets[GlobalCollections.Targets.Count - 1] != _hit.transform) {
                            //Path
                            var targets = GlobalCollections.Targets;
                            var last_target_pos = targets[targets.Count - 1].transform.position;
                            var segment = new Segment(last_target_pos, _hit.point);
                            
                            GlobalCollections.Path.OpenPath();
                            GlobalCollections.Path.Add(segment);
                            GlobalCollections.Path.ClosePath();
                            
                            //Targets
                            GlobalCollections.Targets.Add(_hit.transform);
                        }
                    }
                }
            }

            if (_hit.point == Vector3.zero)
                _hit.point = ray.origin + ray.direction * ArrowStats.AttackStats.MarkTargets.RangeOnNoHit;
        }

        public static void Draw() {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)).origin,  _hit.point);
            Gizmos.DrawWireSphere(_hit.point, ArrowStats.AttackStats.MarkTargets.Radius);
        }

        // // Update is called once per frame
        // private IEnumerator StartMarking() {
        //     Time.timeScale = slowAmount;
        //     TargetsCollection.Points =  new List<Transform>();
        //     this.lineRenderer.positionCount = 3;
        //     this.sphere.gameObject.SetActive(true);

        //     while (this.currentTime <= this.duration) {
        //         this.Mark();
        //         this.DrawMarkingLine();
        //         yield return new WaitForEndOfFrame();

        //         this.currentTime += Time.unscaledDeltaTime;
        //     }
            
            
        //     this.sphere.gameObject.SetActive(false);
        //     this.lineRenderer.positionCount = 0;
        //     this.currentTime = 0f;
        //     Time.timeScale = 1f;
        //     StartCoroutine((shootScript as ArrowMovement).Move());
        // }

    }
}
