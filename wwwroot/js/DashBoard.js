
/*
// 定義 addClickListener 函數
function addClickListener(elementId, callback) {
    const element = document.getElementById(elementId);
    if (element) {
        element.addEventListener("click", callback);
    }
}

// 發送訊息處理
addClickListener("sendMessageBtn", function () {
    const messageInput = document.getElementById("Message");
    const message = messageInput ? messageInput.value : "";

    if (!message) {
        alert("請輸入有效的訊息內容！");
        return;
    }

    const sendButton = document.getElementById("sendMessageBtn");
    sendButton.disabled = true;
    sendButton.innerText = "發送中...";

    const xhr = new XMLHttpRequest();
    xhr.open("POST", "/Backend/SendEmail", true); // 更新為正確的 API 路徑
    xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");

    xhr.onload = function () {
        sendButton.disabled = false;
        sendButton.innerText = "發送郵件";

        if (xhr.status === 200) {
            alert("郵件發送成功！");
            if (messageInput) messageInput.value = "";
        } else {
            alert("郵件發送失敗：" + xhr.responseText);
        }
    };

    const data = "message=" + encodeURIComponent(message) + "&actionType=yes";
    xhr.send(data);
});
*/