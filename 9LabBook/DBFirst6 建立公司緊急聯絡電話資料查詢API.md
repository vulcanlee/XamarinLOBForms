# DBFirst6 建立公司緊急聯絡電話資料查詢API

在這裡，我們將會建立一個控制器，並且只會設計一個 Http GET 動作，這個動作將會回傳所有公司緊急聯絡電話資料。

## 建立公司緊急聯絡電話資料控制器

* 滑鼠右擊 `Controllers` 資料夾，選擇 `加入` > `控制器`

  ![](Images/DBFirst25.png)

* 在 `新增 Scaffold` 對話窗中，點選 `Web API 2 控制器 - 空白` > `新增`

* 在 `加入控制器` 對話窗中，輸入 `OnCallPhone`，如同底下畫面，最後點選 `加入` 按鈕

  ![](Images/AddController1.png)

* 新增的類別檔案最上方，加入參考這些命名空間

![](Icons/csharp.png)

```csharp
using LOBFormDBFirst.Models;
```

* 將新增的類別以底下程式碼替換

![](Icons/csharp.png)

```csharp
public class OnCallPhoneController : ApiController
{
    private LOBFormEntities db = new LOBFormEntities();
    private APIResult apiResult = new APIResult();
    public APIResult Get()
    {
        return new APIResult()
        {
            Success = true,
            Message = "",
            TokenFail = false,
            Payload = db.LOBOnCallPhones.Select(x => new OnCallPhone()
            {
                OnCallPhoneId = x.OnCallPhoneId,
                PhoneNumber = x.PhoneNumber,
                SortingOrder = x.SortingOrder,
                Title = x.Title,
            })
        };
    }
 
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            db.Dispose();
        }
        base.Dispose(disposing);
    }
}
```

# 測試此控制器

* 執行此專案

* 打開 PostMan 工具

  * 選擇 Http 方法為 `Get`

  * 輸入 URL 為 `http://localhost:50490/api/OnCallPhone`

    > 若您自己建立的 Web API 專案，請在這裡輸入您專案的 Port 編號

  * 點選 `Send` 按鈕

![PostMan](Images/DPostMan2.png)

* 若輸出底下內容，則表示 `公司緊急聯絡電話資料` 控制器，已經成功建立完成了

![](Icons/Json.png)

```json
{
    "success": true,
    "tokenFail": false,
    "message": "",
    "payload": [
        {
            "onCallPhoneId": 1,
            "sortingOrder": 0,
            "title": "警衛室",
            "phoneNumber": "+8860912345670"
        },
        ...
        {
            "onCallPhoneId": 17,
            "sortingOrder": 16,
            "title": "業務部 Sales Department",
            "phoneNumber": "+8860912345676"
        }
    ]
}
```

# 問題研究

![](Icons/fa-question-circle30.png) 在這個練習中，為何要選取 `Web API 2 控制器 - 空白` 這個控制器產生範本

![](Icons/fa-question-circle30.png) 此敘述 `Context.OnCallPhones.ToList()` ，在執行上發生了甚麼事情？

![](Icons/fa-question-circle30.png) 為何這個 Get 動作 (Action) `public APIResult Get()`，會指定回傳 APIResult 這個類別物件。

![](Icons/fa-question-circle30.png) 任何人是否都可以呼叫這個 `http://localhost:50490/api/OnCallPhone` Web API


