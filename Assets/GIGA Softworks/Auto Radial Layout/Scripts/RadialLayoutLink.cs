using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


namespace GIGA.AutoRadialLayout
{
    public class RadialLayoutLink : MonoBehaviour
    {
        public enum ProgressMode {Instant,Animated };

        internal RadialLayoutLinksRoot root;

        [ReadOnly]
        public RadialLayoutNode from;
        [ReadOnly]
        public RadialLayout fromLayout;
        [ReadOnly]
        public RadialLayoutNode to;
        public Image progress;
        private float targetProgressValue;
        public float ProgressValue { 
            get {
                return this.targetProgressValue;
            }
            set {
                this.targetProgressValue = value;
                if (this.root.parentLayout.linksProgressMode == ProgressMode.Instant || !Application.isPlaying)
                {
                    this.progress.fillAmount = this.GetRealFillValue(this.targetProgressValue);
                    OnProgressChanged?.Invoke(this.targetProgressValue);
                    OnProgressCompleted?.Invoke(this.targetProgressValue);
                }
                else
                {
                    if (this.animCoroutine != null)
                        this.StopCoroutine(animCoroutine);
                    this.animCoroutine = StartCoroutine(ProgressCoroutine(this.targetProgressValue,this.root.parentLayout.linksProgressSpeed/50f));
                }
            }
        }

        public UnityEvent<float> OnProgressChanged = null;
        public UnityEvent<float> OnProgressCompleted = null;

        private Coroutine animCoroutine = null;

        /// <summary>
        /// Sets the node starting/ending points.
        /// </summary>
        public void Set(RadialLayoutNode from, RadialLayoutNode to)
        {
            this.fromLayout = null;
            this.from = from;
            this.to = to;

            this.name = $"link_{from.name}->{to.name}";

            // Setting position & rotation
            RectTransform rt = this.GetComponent<RectTransform>();
            rt.position = from.GetComponent<RectTransform>().position;

            Vector3 toVector = to.GetComponent<RectTransform>().position - from.GetComponent<RectTransform>().position;
            rt.rotation = Quaternion.LookRotation(toVector, Vector3.up);
            if (toVector.x >= 0)
                rt.eulerAngles = new Vector3(0, 0, -rt.eulerAngles.x);
            else
                rt.eulerAngles = new Vector3(0, 0, 180 + rt.eulerAngles.x);

            rt.sizeDelta = new Vector2(toVector.magnitude / from.Layout.Canvas.scaleFactor, rt.sizeDelta.y);
        }

        /// <summary>
        /// Sets the node starting/ending points.
        /// </summary>
        public void Set(RadialLayout from, RadialLayoutNode to)
        {
            this.fromLayout = from;
            this.from = null;
            this.to = to;

            this.name = $"link_{from.name}->{to.name}";

            // Setting position & rotation
            RectTransform rt = this.GetComponent<RectTransform>();
            rt.position = from.GetComponent<RectTransform>().position;

            Vector3 toVector = to.GetComponent<RectTransform>().position - from.GetComponent<RectTransform>().position;
            if(toVector != Vector3.zero)
                rt.rotation = Quaternion.LookRotation(toVector, Vector3.up);
            if (toVector.x >= 0)
                rt.eulerAngles = new Vector3(0, 0, -rt.eulerAngles.x);
            else
                rt.eulerAngles = new Vector3(0, 0, 180 + rt.eulerAngles.x);

            rt.sizeDelta = new Vector2(toVector.magnitude / from.Canvas.scaleFactor, rt.sizeDelta.y);
        }

        private IEnumerator ProgressCoroutine(float target, float speed)
        {
            bool finished = false;
            float realTargetValue = this.GetRealFillValue(this.targetProgressValue);
            while (!finished)
            {
                if (this.progress.fillAmount < realTargetValue)
                {
                    this.progress.fillAmount += Time.deltaTime * speed;
                    if (this.progress.fillAmount >= realTargetValue || this.progress.fillAmount >= 1)
                    {
                        this.progress.fillAmount = Mathf.Min(1, realTargetValue);
                        finished = true;
                    }
                }
                else if (this.progress.fillAmount > realTargetValue)
                {
                    this.progress.fillAmount -= Time.deltaTime * speed;
                    if (this.progress.fillAmount <= realTargetValue || this.progress.fillAmount <= 0)
                    {
                        this.progress.fillAmount = Mathf.Max(0, realTargetValue);
                        finished = true;
                    }
                }
                else
                    finished = true;

                this.OnProgressChanged?.Invoke(this.GetRealFillValue(this.progress.fillAmount));

                yield return null;
            }

            if (finished && this.OnProgressCompleted != null)
                this.OnProgressCompleted.Invoke(this.GetRealFillValue(this.targetProgressValue));

            this.animCoroutine = null;
        }

        private float GetRealMinFillValue()
        {
            // Since the link is partially covered by the node, returning the real 0% based on node radius (from)
            if (this.from != null)
            {
                return (this.from.nodeRadius * this.from.transform.localScale.x ) / this.GetComponent<RectTransform>().sizeDelta.x;
            }
            else if (this.fromLayout != null)
            {
                if (this.fromLayout.IsSubLayout)
                {
                    RadialLayoutNode node = this.fromLayout.GetComponentInParent<RadialLayoutNode>();
                    return (node.nodeRadius * node.transform.localScale.x ) / this.GetComponent<RectTransform>().sizeDelta.x;
                }
                else if(fromLayout.showInnerNode)
                {
                    return (fromLayout.innerNode.GetComponent<RadialLayoutInnerNode>().nodeRadius * fromLayout.innerNode.transform.localScale.x ) / this.GetComponent<RectTransform>().sizeDelta.x;
                }
            }

            return 0;
        }

        private float GetRealMaxFillValue()
        {
            // Since the link is partially covered by the node, returning the real 100% based on node radius (to)
            if (this.to != null)
            {
                return 1 - ((this.to.nodeRadius * this.to.transform.localScale.x ) / this.GetComponent<RectTransform>().sizeDelta.x);
            }
            
            return 0;
        }

        /// <summary>
        /// Since Links are partially covered by Nodes, this function returns the real fill amount to be used when applying progress to the Link image fill.
        /// The boundaries are defined by the nodeRadius property of the Node.
        /// </summary>
        /// <param name="target">The target fill percentage.</param>
        /// <returns>The adjusted percentage that fits in the real link fill length.</returns>
        public float GetRealFillValue(float target)
        {
            float realMin = this.GetRealMinFillValue();
            float realMax = this.GetRealMaxFillValue();
            return realMin + (target * (realMax - realMin));
        }
    }
}
