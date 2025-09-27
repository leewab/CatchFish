namespace Game
{
    /// <summary>
    /// 组件创建者接口
    /// 实现该接口的类提供了Make组件的相关方法
    /// </summary>
    public interface IComponentMaker<C> where C : GameComponent
    {
        /// <summary>
        /// 创建(C#实现的)基础组件，并添加到自身的成员当中
        /// </summary>
        /// <param name="type">组件类型名称</param>
        /// <param name="path">组件的路径</param>
        /// <returns>创建出来的组件</returns>
        C LMake(string type, string path);

        /// <summary>
        /// 创建(C#实现的)基础组件，并添加到自身的成员当中
        /// </summary>
        /// <typeparam name="T">组件的类型</typeparam>
        /// <param name="path">组件的路径</param>
        /// <returns>创建出来的组件</returns>
        C LMake<T>(string path) where T : GameComponent;

        /// <summary>
        /// 创建Lua实现的复合组件，并添加到自身的成员当中
        /// </summary>
        /// <param name="table">组件对应的LuaTable</param>
        /// <param name="path">组件的路径</param>
        /// <returns>创建出来的组件</returns>
        C LMakeLuaComponent( string path);
    }
}