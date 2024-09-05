using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DeadFusion.Utility
{
    [System.Serializable]
    public class BoolOverrideList
    {
        [System.Serializable]
        public class BoolOverride
        {
            public string label;
            public bool value;
        }

        [SerializeField]
        private List<BoolOverride> bools;

        public bool isAnyActive { get; private set; }

        public bool isEveryActive { get; private set; }


        public void SetBool(string label, bool value)
        {
            int index = GetIndex(label);

            if (index == -1)
                bools.Add(new BoolOverride { label = label, value = value });
            else
            {
                if (bools[index].value == value)
                    return;
                bools[index].value = value;
            }

            UpdateBools();
        }

        public IEnumerator SetBoolWithDelayCoroutine(string label, bool value, float delay)
        {
            yield return new WaitForSeconds(delay);
            SetBool(label, value);
            UpdateBools();
        }

        private int GetIndex(string label)
        {
            for (int i = 0; i < bools.Count; i++) 
            {
                if (bools[i].label == label)
                    return i;
            }
            return -1;
        }

        private void UpdateBools()
        {
            bool anyActive = false;
            bool everyActive = true;

            for (int i = 0; i < bools.Count; i++)
            {
                if (bools[i].value)
                {
                    anyActive = true;
                    if (!everyActive)
                        break;
                }
                else
                {
                    everyActive = false;
                    if (anyActive)
                        break;
                }
            }

            isAnyActive = anyActive;
            isEveryActive = everyActive;
        }
    }
}