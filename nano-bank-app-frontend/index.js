function onloadBody(){
    document.querySelector('.dest-acc-num').style.visibility = 'hidden';
    document.querySelectorAll('.acc-data')[1].hidden = true;
    document.querySelector('.alert-danger').hidden = true;
    document.querySelector('.alert-primary').hidden = true;
    accDataSingleMulti('100%');
    listAllPaymentActionRecords()
}


let globalActiveAction = 'Deposit';

function actionSelect(evCaller){
    console.log(evCaller.value);
    globalActiveAction = evCaller.value;
    if(evCaller.value === 'Transaction'){
        document.querySelector(".dest-acc-num").style.visibility = 'visible';
        document.querySelectorAll('.acc-data')[1].hidden = false;
        accDataSingleMulti('49%')
    }
    else {
        document.querySelector(".dest-acc-num").style.visibility = 'hidden';
        document.querySelectorAll('.acc-data')[1].hidden = true;
        accDataSingleMulti('100%');
    }
    console.log(globalActiveAction);
}


function accDataSingleMulti(value){
    document.querySelectorAll('.acc-data').forEach( el => {
        el.style.width = value;
    })
}

function onchangeAccDest(evCaller){
    console.log('dest acc fetch');
    fetch('http://localhost:3000/getBankAccount/?accnumber=' + evCaller.value, {
    method: "GET",
  }).then((p) =>
    p.json().then((data) => {
      console.log("data recv", data);
        let acc1 = document.querySelector('.acc-2');
        acc1.querySelector('.li-id').innerHTML = 'Id<br>'+ data.Id;
        acc1.querySelector('.li-name').innerHTML = 'Name<br>'+data.Name;
        acc1.querySelector('.li-balance').innerHTML = 'Balance<br>'+data.Balance;
        acc1.querySelector('.li-address').innerHTML ='Account address<br>'+ data.BankAccountAddress;
        acc1.querySelector('.li-status').innerHTML = 'Status<br>'+data.Status;
     
    })
  );
}

function onchangeAccSrc(evCaller){
    console.log('src acc fetch');
    fetch('http://localhost:3000/getBankAccount/?accnumber=' + evCaller.value, {
    method: "GET",
  }).then((p) =>
    p.json().then((data) => {
      console.log("data recv", data);
        let acc1 = document.querySelector('.acc-1');
        acc1.querySelector('.li-id').innerHTML = 'Id<br>'+ data.Id;
        acc1.querySelector('.li-name').innerHTML = 'Name<br>'+data.Name;
        acc1.querySelector('.li-balance').innerHTML = 'Balance<br>'+data.Balance;
        acc1.querySelector('.li-address').innerHTML ='Account address<br>'+ data.BankAccountAddress;
        acc1.querySelector('.li-status').innerHTML = 'Status<br>'+data.Status;
     
    })
  );
}

function listAllPaymentActionRecords(){
    fetch('http://localhost:3000/listPaymentRecords', {
    method: "GET",
  }).then((p) =>
    p.json().then((data) => {
        console.log("data recv", data);
        let tblBody = document.querySelector("tbody");
        tblBody.innerHTML = "";

        data.records.reverse().forEach(el => {
            let row = document.createElement("tr");
            let cell = document.createElement("td");
            cell.appendChild(document.createTextNode(el.Id));
            row.appendChild(cell);
            cell = document.createElement("td");
            cell.appendChild(document.createTextNode(el.ActionType__c));
            row.appendChild(cell);
            cell = document.createElement("td");
            cell.appendChild(document.createTextNode('$'+el.Amount__c));
            row.appendChild(cell);
            cell = document.createElement("td");
            cell.appendChild(document.createTextNode(el.SourceAddress__c));
            row.appendChild(cell);
            cell = document.createElement("td");
            cell.appendChild(document.createTextNode(el.DestinationAddress__c));
            row.appendChild(cell);
            cell = document.createElement("td");
            cell.appendChild(document.createTextNode(el.CreatedDate));
            row.appendChild(cell);
            tblBody.appendChild(row);
        })
    })
  );
}

function executeAction(){
    console.log(globalActiveAction);
    if(globalActiveAction == 'Transaction')
        transact();
    else if (globalActiveAction == 'Deposit')
        deposit();
    else if (globalActiveAction == 'Withdraw')
        withdraw();
}

function deposit(){
    let srcAddr = document.querySelector('.src-input').value;
    let amount = document.querySelector('.amount-input').value;
    console.log('deposit',srcAddr,amount);
    if(srcAddr == undefined || srcAddr == null || srcAddr == "")    
        return;
    if(amount == undefined || amount == null || amount == "")
        return;

    depositFetch(srcAddr, amount);
}

function withdraw(){
    let srcAddr = document.querySelector('.src-input').value;
    let amount = document.querySelector('.amount-input').value;
    console.log('withdraw',srcAddr,amount);
    if(srcAddr == undefined || srcAddr == null || srcAddr == "")    
        return;
    if(amount == undefined || amount == null || amount == "")
        return;

    withdrawFetch(srcAddr, amount);
}

function transact(){
    let srcAddr = document.querySelector('.src-input').value;
    let destAddr = document.querySelector('.dest-input').value;
    let amount = document.querySelector('.amount-input').value;
    console.log('transaction',srcAddr,destAddr,amount);
    if(srcAddr == undefined || srcAddr == null || srcAddr == "")    
        return;
    if(amount == undefined || amount == null || amount == "")
        return;
    if(destAddr == undefined || destAddr == null || destAddr == "")
        return;
    if(srcAddr == destAddr)
        return;

    transactionFetch(srcAddr,destAddr, amount);
}

function withdrawFetch(address, amount){
    console.log('fetch', address,amount)
    fetch('http://localhost:5055/Withdraw', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: `{
                    "bankAccountAddress": "${address}",
                    "amount": ${amount}
                }`,
    })
    .then(response => response.json())
    .then(data => {
        console.log('Success:', data);
        let tmpAdr = { value: address };
        onchangeAccSrc(tmpAdr);
        SuccessfullRequest("Withdraw action successfully to executed!");
    })
    .catch((error) => {
        console.error('Error:', error);
        failedRequest("Withdraw action failed to execute!");
    });
}


function depositFetch(address, amount){
    console.log('fetch', address,amount)
    fetch('http://localhost:5055/Deposit', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: `{
                    "bankAccountAddress": "${address}",
                    "amount": ${amount}
                }`,
    })
    .then(response => response.json())
    .then(data => {
        console.log('Success:', data);
        let tmpAdr = { value: address };
        onchangeAccSrc(tmpAdr);
        SuccessfullRequest("Deposit action successfully to executed!");
    })
    .catch((error) => {
        console.error('Error:', error);
        failedRequest("Deposit action failed to execute!");
    });
}

function transactionFetch(srcAddress, destAddress, amount){
    console.log('fetch', srcAddress, destAddress, amount)
    fetch('http://localhost:5055/Transaction', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: `{
                    "sourceAccountAddress": "${srcAddress}",
                    "destinationAccountAddress": "${destAddress}",
                    "amount": ${amount}
                }`,
    })
    .then(response => response.json())
    .then(data => {
        console.log('Success:', data);
        let tmpAdrS = { value: srcAddress };
        onchangeAccSrc(tmpAdrS);
        let tmpAdrD = { value: destAddress };
        onchangeAccDest(tmpAdrD);
        SuccessfullRequest("Transaction action successfully to executed!");
    })
    .catch((error) => {
        console.error('Error:', error);
        failedRequest("Transaction action failed to execute!");
    });
}

function failedRequest(text){
    document.querySelector('.alert-danger').innerHTML = text;
    document.querySelector('.alert-danger').hidden = false;
    setTimeout(() => { 
        document.querySelector('.alert-danger').hidden = true;
    }, 2000);
}

function SuccessfullRequest(text){
    document.querySelector('.alert-primary').innerHTML = text;
    document.querySelector('.alert-primary').hidden = false;
    listAllPaymentActionRecords();
    setTimeout(() => { 
        document.querySelector('.alert-primary').hidden = true;
    }, 2000);
}