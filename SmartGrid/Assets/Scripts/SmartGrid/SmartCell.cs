using UnityEngine;
namespace SmartGrid
{

    public class SmartCell : IAStarGridCell
    {
        private Vector3 _localPosition;
        public Vector3 LocalPosition
        {
            get { return _localPosition; }
        }
        public bool IsOccupied { get; private set; }

        public float G { get; set; }
        public float H { get; set; }
        public float F { get { return G + H; } }
        public IAStarGridCell Previous { get; set; }
        public float OffsetY = 0;

        private float _edge = 1;

        private IGridIteractiveItem _iteractiveItem;
        public GameObject OccupiedBy { get; private set; }
        private SmartGridController _owner;
        private int i, j;

        public int X { get { return i; } }
        public int Y { get { return j; } }
        public SmartCell(Vector3 localPosition, float edge, SmartGridController grid, int i, int j)
        {
            _localPosition = localPosition;
            _edge = edge;
            _owner = grid;
            this.i = i;
            this.j = j;

        }


        // This function can be called by SmartGridController
        public void UpdateOccupationStatus()
        {
            var objectsInside = Physics.BoxCastAll(_localPosition, new Vector3(_edge / 2, 0, _edge / 2), Vector3.up, Quaternion.identity, Mathf.Infinity, LayerMask.GetMask(LayerMask.LayerToName(0)), QueryTriggerInteraction.Ignore);

            if (objectsInside.Length > 0)
            {

                foreach (var item in objectsInside)
                {

                    if (item.transform.GetComponent<IGridIteractiveItem>() != null)
                    {

                        if (_iteractiveItem == null)
                        {
                            OccupiedBy = item.transform.gameObject;

                            _iteractiveItem = item.transform.GetComponent<IGridIteractiveItem>();

                            _iteractiveItem.SetGrid(_owner);
                            _iteractiveItem.X = X;
                            _iteractiveItem.Y = Y;
                            item.transform.position = new Vector3(_localPosition.x, OffsetY, _localPosition.z);
                            IsOccupied = true;
                            break;
                        }


                    }

                }
                IsOccupied = true;


            }
            else
            {

                _iteractiveItem = null;

                IsOccupied = false;
            }

        }
        public void UpdatePosition(Vector3 position)
        {
            _localPosition = position;
        }
        public void ForceOccupation(object caller, GameObject occupant)
        {
            if (caller is SmartGridController)
            {
                IsOccupied = true;
                OccupiedBy = occupant;
            }

            else
                Debug.LogWarning("Edit not allowed");
        }
        public void DebugDraw()
        {


            if (IsOccupied)
            {
                Gizmos.color = Color.red;
            }
            else
            {
                Gizmos.color = Color.green;
            }

            Gizmos.DrawCube(_localPosition, new Vector3(_edge, 0, _edge));

        }
        public void DebugDraw(Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawCube(_localPosition, new Vector3(_edge, 0, _edge));

        }

        public void Free()
        {
            if (_iteractiveItem != null)
            {
                _iteractiveItem = null;
                OccupiedBy = null;
                IsOccupied = false;

            }
        }


    }

}