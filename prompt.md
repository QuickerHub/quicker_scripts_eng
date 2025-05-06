我来描述一下 .net 程序的执行方式，这是一个浏览器扩展，会根据 .net 程序发送的消息，执行对应的浏览器后台脚本.
例如 api_tabs_create 在浏览器扩展中就是，chrome.tabs.create()
然后 .net 里面有一个统一方法，例如叫
object CallChrome(string api_name, object params);
你只需要用 .net 实现每个 chrome.tabs.create 这样的方法，内部实现传递参数给一个统一方法，用 caller member name 来转换要调用的方法名
然后统一的方法：CallChrome 里面会处理这个api的调用

...
