/* eslint-disable no-unused-vars */
import { useEffect, useState } from 'react';
import './App.css';
import { Moneybox } from './components/moneybox/moneybox';
import { MoneyBoxesService, GlobalAmount } from '../src/services/MoneyBoxService';
import axios from 'axios';
import Button from './components/button/button';

function App() {
    const [moneyBoxes, setMoneyBoxes] = useState();
    const [globalAmountValue, setGlobalAmountValue ] = useState(); 
    function getMoneyBoxes() {
        MoneyBoxesService()
            .then((response) => {
                setMoneyBoxes(response);
            })
            .catch((err) => {
                console.log("App error-> ", err);
            });
    }
    function GetGlobalAmount() {
        GlobalAmount()
            .then((response) => {
                console.log("getglobalamount: ", response)
                setGlobalAmountValue(response);
            })
            .catch((err) => {
                console.log("App error-> ", err);
            });
    }

    useEffect(() => {
        getMoneyBoxes();
        GetGlobalAmount();
    }, []);



    const deposit = () => {
        //console.log("ingresar prueba");
        let value = prompt("Please enter a value");
        if (value.trim() != "" && value.trim() > 0) {
            console.log("correcto")
            axios({
                method: 'post',
                url: `https://localhost:7036/moneyboxes/deposit`,
                headers: {
                    'Content-Type': 'application/json',
                },
                data: value
            }).then(function (response) {
                if (response.status === 200) {
                    getMoneyBoxes();
                    GetGlobalAmount();
                    return Promise.resolve(response.data);
                } else {
                    console.log("Respuesta de red OK pero respuesta HTTP no OK");
                }
            }).catch(function (err) {
                console.log(err, "response")
            });
        }
    }
    const singleDeposit = (e) => {
        //console.log("ingresar prueba");
        let value = prompt("Please enter a value");
        if (value.trim() != "" && value.trim() > 0) {
            console.log("correcto")
            axios({
                method: 'post',
                url: `https://localhost:7036/moneyboxes/singledeposit/${e}`,
                headers: {
                    'Content-Type': 'application/json',
                },
                data: value
            }).then(function (response) {
                if (response.status === 200) {
                    getMoneyBoxes();
                    GetGlobalAmount();
                    return Promise.resolve(response.data);
                } else {
                    console.log("Respuesta de red OK pero respuesta HTTP no OK");
                }
            }).catch(function (err) {
                console.log(err, "response")
            });
        }
    }
    const withdraw = (e) => {
        let value = prompt("Please enter a value");
        if (value.trim() != "" && value.trim() > 0) {
            console.log("correcto")
            axios({
                method: 'post',
                url: `https://localhost:7036/moneyboxes/withdraw/${e}`,
                headers: {
                    'Content-Type': 'application/json',
                },
                data: value
            }).then(function (response) {
                if (response.status === 200) {
                    getMoneyBoxes();
                    GetGlobalAmount();
                    return Promise.resolve(response.data);
                } else {
                    console.log("Respuesta de red OK pero respuesta HTTP no OK");
                }
            }).catch(function (err) {
                console.log(err, "response")
            });
        }
    }


    return (
        <div className="app-root">
            <div className="header">
                <h1>Facturación</h1>
                <h2>Global Amount: {globalAmountValue}&euro;</h2>
                <Button action={deposit} />
            </div>
            <div className="squares-container">
                {moneyBoxes &&
                    moneyBoxes.map((moneyBox, i) => {
                        return (
                            <Moneybox key={i} moneyBoxData={moneyBox} withdraw={withdraw} singleDeposit={singleDeposit} />
                        )
                    })
                }
            </div>
        </div>
    );


}

export default App;