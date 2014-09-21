# Translate IP for Storefront

## Intro
storefront 默认是没有类似 Webinterface 的 secure access 中配置 translate 方法来做地质转换的功能。所以，我就写TranslateIP 来给 SF 增加一个类似的功能。

---
为了简单呢，没有使用 web.config 文件进行配置，也避免了对原有站点的破坏，所以，配置文件直接放在了%system% 下面

## Build
由于使用的是 VS2013，所以项目可以直接编译，没有其他依赖。
如果是较老的版本，可以直接创建项目并复制大部分 Translate.cs 代码即可

## Misc
 其实 Citrix 也有提供 Storefront SDK，可以基于 SDK 来处理 ICA 文件，并根据规则进行替换，但是考虑到这个功能比较简单，只使用了基本的 IIS 对 HTTP 请求的处理来进行替换，其实有点 dirty hack，没有使用正规的 SDK。
至于应该怎么做，还请大家自己权衡。
