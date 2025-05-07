# Quicker Scripts Engine

这个仓库包含用于Quicker的脚本引擎和Chrome API模拟实现。

[Quicker浏览器控制 - 后台命令参考](https://quickerconnectortests.getquicker.cn/docs/commands.html#browser-scripts)



## 项目结构

- **ConsoleApp1** - Chrome API桥接回调测试程序
- **chrome_scripts** - 用于Chrome浏览器的脚本集合

## ConsoleApp1 功能介绍

ConsoleApp1是一个用于测试Chrome扩展API桥接功能的控制台应用程序。该应用程序模拟了Chrome浏览器扩展API的行为，使开发人员能够在不启动浏览器的情况下测试Chrome扩展脚本。

### 主要功能

- **Chrome API模拟** - 通过ChromeController提供Chrome浏览器扩展API的模拟实现
- **JavaScript执行环境** - 提供可执行JavaScript的脚本引擎
- **回调函数支持** - 支持Chrome API中常见的异步回调模式
- **API测试** - 内置多种Chrome API的测试用例，包括：
  - `tabs.query` - 查询浏览器标签页
  - `tabs.create` - 创建新标签页
  - `tabs.get` - 获取标签页信息
  - `browsingData` - 浏览数据管理
  - `cookies` - Cookie管理
  - `downloads` - 下载管理
  - `history` - 历史记录
  - `tts` - 文本转语音

### 技术实现

该项目由两个主要组件组成：

1. **ChromeController** - 提供Chrome API的模拟实现
   - ScriptEngineWrapper - JavaScript脚本引擎封装
   - ChromeBridge - Chrome API桥接实现

2. **ConsoleApp1** - 测试应用程序
   - 提供测试用例和演示代码

## 使用方法

运行ConsoleApp1可以查看Chrome API桥接的测试结果。应用程序会执行一系列测试脚本，并在控制台输出结果。
