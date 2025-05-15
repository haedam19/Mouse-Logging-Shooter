using MouseLog;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target3D : MonoBehaviour
{
    #region Fields
    TrialData _tdata = null; // 자신이 타겟이 되는 trial. 몇 번째 trial의 타겟인지 확인하는 데에 사용. 
    ConditionData _cdata = null; // 속한 condition. 총 trial 수를 확인하는 데에 사용.

    [SerializeField] Material m_onMaterial;
    [SerializeField] Material m_offMaterial;
    MeshRenderer m_renderer;
    SphereCollider m_collider;

    float radius;
    public Vector2 posOnScreen;
    #endregion

    #region Properties: Radius, Center
    public float Radius
    {
        get { return radius; }
        set
        {
            radius = value;
            transform.localScale = new Vector3(0.1f * value, 0.1f * value, 0.1f * value);
        }
    }

    public PointR Center { get { return new PointR(posOnScreen.x, posOnScreen.y); } }
    #endregion

    void Awake()
    {
        m_renderer = GetComponent<MeshRenderer>();
        m_collider = GetComponent<SphereCollider>();
        TargetOff();
    }

    public void TargetOn()
    {
        m_renderer.material = m_onMaterial;
        m_collider.enabled = true;
    }

    public void TargetOff()
    {
        m_renderer.material = m_offMaterial;
        m_collider.enabled = false;
    }

    public bool Contains(PointR p)
    {
        return PointR.Distance(Center, p) <= radius;
    }

    public void BindCondition(ConditionData cdata) { _cdata = cdata; }
    public void BindTrial(TrialData tdata) { _tdata = tdata; }
}
