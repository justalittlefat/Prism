## 简介
#### 这是一个Unity AssetBundle管理模块

> * 在开发流程中：AssetBundle Browser面板可以配置资源筛选规则，然后根据筛选规则自动定位目标资源，分析依赖，最终实现一键多平台打包
> * 在运行过程中：BundleManager负责提供资源同步和异步加载API，并监视资源的依赖和引用计数，实现自动卸载

## AssetBundle Browser
* ![image](https://github.com/justalittlefat/Prism/blob/master/Images/01.jpg)
* 菜单栏 prism > AssetBundle Browser
* 在官方的AssetBundle Browser基础上增加了两个页面
* 主要功能为根据筛选规则自动设置assetbundle名，以及自动打包多平台bundle

## BundleManager
* 脚本位于：_Frameworks/Prism/Scripts/BundleManager，需载到gameobject上，并调用init初始化
* BundleManager提供同步加载和异步加载API
    > public GameObject LoadPrefab(string path, bool enable = true);  <br/>
    > public void LoadPrefabAsync(string path, Action<GameObject> onLoadPrefabComplete);
* 根据引用计数和依赖计数进行自动资源管理
* 当使用资源的gameobject都被销毁后，内存中的资源将逐步为0
* ![image](https://github.com/justalittlefat/Prism/blob/master/Images/02.jpg)
