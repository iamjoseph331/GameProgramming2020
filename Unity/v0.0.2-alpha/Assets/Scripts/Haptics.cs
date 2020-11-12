using System.Collections;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{

    public class Haptics : MonoBehaviour
    {
        public bool on = false;
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            Hand hand = GetComponentInParent<Hand>();
        }
    }
}