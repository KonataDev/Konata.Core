namespace Konata.Core.Base.ComponentType
{
    /// <summary>
    /// 组件正式开始运行时的处理
    /// </summary>
    public interface IStart
    {
        void Start(Entity entity);
    }
}
