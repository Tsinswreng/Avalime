仿照安卓的入口流程 改Windows的入口流程

首先你要準備一份Windows的配置文件

參考
E:\_code\CsRime\Avalime\Avalime.Frontend\proj\Avalime.Android\Assets\Avalime.Ro.jsonc
E:\_code\CsRime\Avalime\Avalime.Frontend\proj\Avalime.Android\Assets\Avalime.Rw.jsonc

但是要放在windows的文件夾裏

其中
rime.dll 路徑在
D:\ENV\Rime\weasel-0.15.0\rime.dll

UserDataDir路徑在
D:\Program Files\Rime\User_Data

然後在Run.sh裏 先把配置文件複製到輸出目錄


2026_0627_111243[
avalime windows有按鍵處理的問題

]
