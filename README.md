# MyETDemo
照着肉饼老师的讲解视频做的独立的ET框架客户端Demo ，这个是ET框架地址：https://github.com/egametang/ET ，框架作者：熊猫，社区参与者 哲学绅士，Justin沙特王子 ，这个是肉饼老师写的独立版的ET框架Demo地址：https://github.com/roubingcode/ETClient ，ET框架相关介绍都在ET框架地址里面，就不做赘述 ,如果导出后运行报错“无法连接到目标主机”，就到ET框架地址里面将“FileServer”放到你的项目外面，比如我的项目文件夹是“MyETDemo”，那么就将“FileServer”文件夹放到项目文件夹外面，接着在Unity场景中的“Tools”工具栏下找到web资源服务器，并打开就可以运行了。
说下我遇到的一个坑，Demo写完后，物体怎么都不会动，然后我找了很久才找到问题，是因为我的Plane的Layer属性不对，你需要新建一个Layer，然后选择你新建的这个Layer，这个Layer的名称还必须是Map，接着再按鼠标右键，物体就可以移动了（目前我也没有找到这个Layer名称为什么一定是Map的原因）
