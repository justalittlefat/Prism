# 练手模块，部分细节未完善，慎用

## 打包辅助工具
* ![image](https://github.com/justalittlefat/Prism/blob/master/Images/01.jpg)
* 菜单栏 prism > AssetBundle Browser
* 在官方的AssetBundle Browser基础上增加了两个页面
* 主要功能为根据筛选规则自动设置assetbundle名，以及自动打包多平台bundle

## BundleManager
* 需挂载到gameobject上，并调用init初始化
* 提供同步加载和异步加载API
* 根据引用计数和依赖计数进行自动资源管理
* 当使用资源的gameobject都被销毁后，内存中的资源将逐步为0
* ![image](https://github.com/justalittlefat/Prism/blob/master/Images/02.jpg)
