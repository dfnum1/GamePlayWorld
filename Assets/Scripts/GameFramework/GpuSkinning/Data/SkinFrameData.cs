#if !USE_SERVER
using UnityEngine;

namespace Framework.Plugin
{
    public interface ISkinAnimData
    {
        string GetName();
        float GetSpeed();
        float GetDuration();
        bool IsLoop();
    }

    public class SkinFrameData
    {
        public string assetFile = "";
        protected Mesh m_shareMesh;
        protected Material m_shareMat;
        protected MaterialRef m_MatRef = null;
        //  public byte         uvOffsetIndex = 0;
        virtual public SkeletonSlot[] getSlots() { return null; }
        virtual public void setSlots(SkeletonSlot[] slots) { }
        public virtual Mesh getShareMesh() { return m_shareMesh; }
        public virtual Material getShareMat() { return m_shareMat; }
        public virtual int getAnimCnt() { return 0; }
        public virtual string getAnimState(int index) { return ""; }
        public virtual Vector2[] getSkins() { return null; }

        MaterialRef.MallocMaterialDefEvent m_pMalloc= null;
        MaterialRef.FreeMaterialDefEvent m_pFree = null;
        private System.Action<SkinFrameData> m_OnReleaseSkinFrameData ;
        public void SetDelegate(System.Action<SkinFrameData> onRlease, MaterialRef.MallocMaterialDefEvent malloc, MaterialRef.FreeMaterialDefEvent free)
        {
            m_pMalloc = malloc;
            m_pFree = free;
            m_OnReleaseSkinFrameData = onRlease;
        }

        public virtual Material getShareMat(Texture texture)
        {
            if (m_shareMat != null) return m_shareMat;
            if (m_pMalloc == null)
                return null;
            m_MatRef = m_pMalloc(texture);
            if (m_MatRef.material == null)
                m_MatRef.material = getShareMat();
            m_MatRef.refIndex++;
            return m_MatRef.material;
        }


        private int m_refIndex = 0;
        public int refIndex
        {
            get { return m_refIndex; }
        }
        public void grab()
        {
            m_refIndex++;
        }

        public void release()
        {
            m_refIndex--;
            if (m_MatRef != null)
            {
                if(m_pFree!=null)
                    m_pFree(m_MatRef);
            }
            if (refIndex <= 0)
                clear();
        }

        public virtual void clear()
        {
            if (m_shareMesh)
            {
#if UNITY_EDITOR
                if(Application.isPlaying)
                    UnityEngine.Object.Destroy(m_shareMesh);
                else
                    UnityEngine.Object.DestroyImmediate(m_shareMesh);
#else
                UnityEngine.Object.Destroy(m_shareMesh);
#endif
            }
            if (m_shareMat)
            {
#if UNITY_EDITOR
                if (Application.isPlaying)
                    UnityEngine.Object.Destroy(m_shareMat);
                else
                    UnityEngine.Object.DestroyImmediate(m_shareMat);
#else
                UnityEngine.Object.Destroy(m_shareMat);
#endif
            }
            m_shareMesh = null;
            m_shareMat = null;
            m_refIndex = 0;
            m_MatRef = null;
            if (m_OnReleaseSkinFrameData != null)
                m_OnReleaseSkinFrameData(this);
            assetFile = "";
        }

        public virtual void clearMesh() { }

        public virtual void dirty()
        {

        }

        public virtual void update()
        {

        }

#if UNITY_EDITOR
        public virtual void setShareMesh(Mesh mesh)
        {
            m_shareMesh = mesh;
        }

        public void setShareMat(Material mat)
        {
            m_shareMat = mat;
        }
#endif
    }
}
#endif