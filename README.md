练手模块，未经过商业项目检验，慎用

---editor
菜单栏 prism > AssetBundle Browser >> folder页
根据筛选规则自动设置assetbundle

---scripts
BundleManager
根据引用计数和依赖计数进行bundle管理的模块
当引用和依赖均为0时自动卸载资源
支持异步加载
需挂载到gameobject上，并调用init初始化
