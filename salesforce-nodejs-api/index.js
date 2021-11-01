const jsforce = require('jsforce');
const express = require('express');
const cors = require('cors');

console.log('jsforce started');
const securityToken = '';
const username = '';
const password = '';

const app = express();
app.use(express.urlencoded({ extended: false }));
app.use(express.json());
app.use(cors());
const port = 3000;

var conn = new jsforce.Connection({
    // you can change loginUrl to connect to sandbox or prerelease env.
    // loginUrl : 'https://test.salesforce.com'
});


conn.login(username, password + securityToken, function(err, userInfo) {

    // http://localhost:3000/getBankAccount/?accnumber=12-6548858-98456
    app.get('/getBankAccount', (req, res) => {
        if (err) { 
            return console.error(err); 
        }
        const acc_address = req.query.accnumber;
        conn.query(`SELECT Id, Name, AccountOwner__c, BankAccountAddress__c, Status__c, Balance__c
                    FROM BankAccount__c 
                    WHERE BankAccountAddress__c = '${acc_address}'`, 
                    function(err, resSF) {
            if (err) {
                return console.error(err);
            }
            console.log(resSF.records[0]);
            let accConverted = {};
            accConverted.Id = resSF.records[0].Id;
            accConverted.Name = resSF.records[0].Name;
            accConverted.Balance = resSF.records[0].Balance__c;
            accConverted.AccountOwner = resSF.records[0].AccountOwner__c;
            accConverted.BankAccountAddress = resSF.records[0].BankAccountAddress__c;
            accConverted.Status = resSF.records[0].Status__c;
            res.send(accConverted);
        });
    });

    // http://localhost:3000/getValidationCreds/?accnumber=12-6548858-98456
    app.get('/getValidationCreds', (req, res) => {
        if(err){
            return console.error(err);
        }
        const acc_address = req.query.accnumber;
        conn.query(`SELECT Id, Pin__c FROM BankAccount__c WHERE BankAccountAddress__c = '${acc_address}'`,
                    function(err, resSF) {
            if(err) {
                return console.error(err);
            }
            console.log(resSF);
            res.send(resSF.records[0]);
        })
    })

    // http://localhost:3000/increaseAccountBalance
    app.put('/increaseAccountBalance', (req, res) => {
        if(err){
            res.sendStatus(400);
            return;
        }
        console.log(req.body.BankAccountAddress, req.body.Amount);
        conn.query(`SELECT Id, Balance__c FROM BankAccount__c WHERE BankAccountAddress__c = '${req.body.BankAccountAddress}'`, function(err, resSF) {
            if (err) {
                res.sendStatus(400);
                return console.error(err);
            }
            let newData = {};
            newData.Balance__c = parseFloat(resSF.records[0].Balance__c) + req.body.Amount;
            newData.Id = resSF.records[0].Id;
            conn.sobject('BankAccount__c').update(newData, function(err, res) {
                if(err){
                    console.log(err)
                }
                if(res){
                    console.log(res)
                }
            })
            res.sendStatus(200);
        });
    })

    // http://localhost:3000/decreaseAccountBalance
    app.put('/decreaseAccountBalance', (req, res) => {
        if(err){
            res.sendStatus(400);
            return;
        }
        console.log(req.body.BankAccountAddress, req.body.Amount);
        conn.query(`SELECT Id, Balance__c FROM BankAccount__c WHERE BankAccountAddress__c = '${req.body.BankAccountAddress}'`, function(err, resSF) {
            if (err) {
                res.sendStatus(400);
                return console.error(err);
            }
            let newData = {};
            newData.Balance__c = parseFloat(resSF.records[0].Balance__c) - req.body.Amount;
            newData.Id = resSF.records[0].Id;
            conn.sobject('BankAccount__c').update(newData, function(err, res) {
                if(err){
                    console.log(err)
                }
                if(res){
                    console.log(res)
                }
            })
            res.sendStatus(200);
        });
    })

    // http://localhost:3000/transactAmount
    app.put('/transactAmount', (req, res) => {
        if(err){
            res.sendStatus(400);
            return;
        }
        console.log(req.body.SourceAccountAddress, req.body.DestinationAccountAddress, req.body.Amount);
        conn.query(`SELECT Id, Balance__c FROM BankAccount__c WHERE BankAccountAddress__c = '${req.body.SourceAccountAddress}'`, function(err, resSF) {
            if (err) {
                res.sendStatus(400);
                return console.error(err);
            }
            let newData = {};
            newData.Balance__c = parseFloat(resSF.records[0].Balance__c) - req.body.Amount;
            newData.Id = resSF.records[0].Id;
            conn.sobject('BankAccount__c').update(newData, function(err, res) {
                if(err){
                    console.log(err)
                    return console.error(err);
                }
                if(res){
                    console.log(res)
                }
            })
        });

        conn.query(`SELECT Id, Balance__c FROM BankAccount__c WHERE BankAccountAddress__c = '${req.body.DestinationAccountAddress}'`, function(err, resSF) {
            if (err) {
                res.sendStatus(400);
                return console.error(err);
            }
            let newData = {};
            newData.Balance__c = parseFloat(resSF.records[0].Balance__c) + req.body.Amount;
            newData.Id = resSF.records[0].Id;
            conn.sobject('BankAccount__c').update(newData, function(err, res) {
                if(err){
                    console.log(err)
                    return console.error(err);
                }
                if(res){
                    console.log(res)
                }
            })
            res.sendStatus(200);
        });
    })

    // http://localhost:3000/paymentAction
    app.post('/paymentAction', (req, res) => {
        console.log(req.body);
        if (err) { 
            res.sendStatus(400); 
            return; 
        }
        console.log(req.body);
        let dataConverted = {};
        dataConverted.ActionType__c = req.body.ActionType;
        dataConverted.Amount__c = req.body.Amount;
        dataConverted.DestinationAddress__c = req.body.DestinationAddress;
        dataConverted.SourceAddress__c = req.body.SourceAddress;
        dataConverted.Name = req.body.Name;
        conn.sobject('PaymentAction__c').create(dataConverted, function (err, ret) {
            if (err || !ret.success) {
                res.sendStatus(400);
                return console.error(err, ret)
            }
            console.log('Created PaymentAction id : ' + ret.id)
            res.sendStatus(200);
        });
    });

    // http://localhost:3000/listAllBankAccounts
    app.get('/listAllBankAccounts', (req, res) => {
        if (err) { 
            return console.error(err); 
        }
        conn.query('SELECT Id, Name, BankAccountAddress__c, Balance__c, Status__c, LastModifiedDate FROM BankAccount__c', function(err, resSF) {
        if (err) {
            return console.error(err);
        }
            //console.log(resSF);
            res.send(resSF);
        });
    });

    // http://localhost:3000/listPaymentRecords
    app.get('/listPaymentRecords', (req, res) => {
        if (err) { 
            return console.error(err); 
        }
        conn.query(`SELECT Id, Name, ActionType__c, Amount__c, SourceAddress__c, DestinationAddress__c,
                    CreatedDate FROM PaymentAction__c ORDER BY CreatedDate`, function(err, resSF) {
        if (err) {
            return console.error(err);
        }
            //console.log(resSF);
            res.send(resSF);
        });
    });
});


app.listen(port, () => {
    console.log(`Api listening on port: ${port}`)
});