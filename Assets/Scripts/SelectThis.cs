using UnityEditor;
using UnityEngine;

public class SelectThis : MonoBehaviour
{
    [SerializeField] SwitchTools.Tools toolType;
    void OnMouseDown()
    {

            switch (toolType)
            {
                case (SwitchTools.Tools.UseGesso):
                {
                    SwitchTools.Instance.UseGesso();
                    break;
                }
                case (SwitchTools.Tools.RemoveGesso):
                    {
                        SwitchTools.Instance.RemoveGesso();
                        break;
                    }
                case (SwitchTools.Tools.UsePaint):
                    {
                        SwitchTools.Instance.UsePaint();
                        break;
                    }
                case (SwitchTools.Tools.UseVarnishRemover):
                    {
                        SwitchTools.Instance.UseVarnishRemover();
                        break;
                    }
                case (SwitchTools.Tools.VisibleLight):
                {
                    SwitchTools.Instance.VisibleLight();
                    break;
                }
                case (SwitchTools.Tools.UVLight):
                    {
                        SwitchTools.Instance.UVLight();
                        break;
                    }
                case (SwitchTools.Tools.InfraredLight):
                    {
                        SwitchTools.Instance.InfraredLight();
                        break;
                    }
                case (SwitchTools.Tools.VarnishSetup):
                    {
                        SwitchTools.Instance.GoToVarnishSetup();
                        break;
                    }

                default:
                        {
                            break;
                        }
            }
            
    }
        
        
}
