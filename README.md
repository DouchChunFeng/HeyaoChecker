<p align="center"><img src="https://raw.githubusercontent.com/DouchChunFeng/HeyaoChecker/main/README_form.png" width="397" alt="图片预览"></p>
<p align="center"><img src="https://raw.githubusercontent.com/DouchChunFeng/HeyaoChecker/main/README_python.png" width="489" alt="图片预览"></p>

# 河妖工房进度通知器
- [功能](#gn)
- [用法](#yf)
- [依赖](#yl)
- [常见问题](#cjwt)
- [注意事项](#zysx)

<a name="gn"></a>
## 功能

- 查询订单变化信息.
- 日志信息保存功能.
- 及时通知到微信消息.

<a name="yf"></a>
## 用法

1. 下载RELEASE中的 `HeyaoChecker.exe` 即可运行.
2. 右键添加添加订单号和通知KEY.
3. 到[SERVER酱官网](https://sct.ftqq.com)获取通知KEY.

<a name="yl"></a>
## 依赖

* [.Net FrameWork 4.0](https://referencesource.microsoft.com)
* [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)
* 本程序使用VS2013编译

<a name="cjwt"></a>
## 常见问题

1. `打开无反应`
- 检查exe文件目录有无[Newtonsoft.Json.dll](https://github.com/JamesNK/Newtonsoft.Json/releases?page=4)文件, 若没有请复制补全即可.

2. `.Net FrameWork 初始化失败`
- 安装[.Net FrameWork 4.0或其他更高的版本](https://www.microsoft.com/zh-cn/download/details.aspx?id=17718)

3. `为什么会产生二个txt文本`
- data.txt -> 用于保存用户信息
- logs.txt -> 用于保存日志信息

<a name="zysx"></a>
## 注意事项

1. **该项目仅供个人使用者参考与学习或使用.**
2. **如侵权，联系删除。侵权等事项与作者无关.**

---