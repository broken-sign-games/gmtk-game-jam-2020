using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK2020.UI
{
    public class TiltBackAndForth : MonoBehaviour
    {
        [SerializeField] private float amplitude = 20f;
        [SerializeField] private float frequency = 1f;

        private void Update()
        {
            transform.localEulerAngles = new Vector3(0, 0, amplitude * Mathf.Sin(frequency * 2 * Mathf.PI * Time.time));
        }
    } 
}
