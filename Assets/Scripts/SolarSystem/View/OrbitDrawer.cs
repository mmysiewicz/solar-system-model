using System;
using UnityEngine;

namespace SolarSystem.View
{
    [RequireComponent(typeof(LineRenderer))]
    public class OrbitDrawer : MonoBehaviour
    {
        [SerializeField] private Orbit _orbit;
        [SerializeField] private int _segments = 120;
        [SerializeField] private float _lineWidth = 0.1f;
        
        private LineRenderer _lineRenderer;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            ConfigureLineRenderer();
        }

        private void Start()
        {
            Draw();
        }

        // LateUpdate wykonuje się tylko po przesunięciu obiektów w Update
        private void LateUpdate()
        {
            Draw();
        }

        private void ConfigureLineRenderer()
        {
            _lineRenderer.useWorldSpace = true;
            _lineRenderer.loop = true;
            _lineRenderer.startWidth = _lineWidth;
            _lineRenderer.endWidth = _lineWidth;

            if (_lineRenderer.sharedMaterial == null)
            {
                _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            }
        }
        
        private void Draw()
        {
            if (_orbit == null || _orbit.getData() == null || _orbit.getCenterPoint() == null)
            {
                return;
            }
            
            var data = _orbit.getData();
            var centerPoint  = _orbit.getCenterPoint().position;

            _lineRenderer.positionCount = _segments;
            for (int i = 0; i < _segments; i++)
            {
                float angle = (i /  (float)_segments) * Mathf.PI * 2f;
                Vector3 relativePosition = OrbitMath.Circular.GetPosition(data.SemiMajorAxis, angle);
                Vector3 rotatedPosition = Quaternion.Euler(data.Inclination, 0, 0) *  relativePosition;
                _lineRenderer.SetPosition(i, centerPoint + rotatedPosition);
            }
        }
    }
}