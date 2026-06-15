import axios from 'axios';
//import { useState } from 'react'
export function MoneyBoxesService() {
    const publicBase = import.meta.env.VITE_API_URL || "https://localhost:7036";
    return axios({
        method: 'get',
        url: `${publicBase}/moneyboxes`,
        headers: {
            'Content-Type': 'application/json',
        }
    })
        .then(function (response) {
            if (response.status === 200) {
                return Promise.resolve(response.data);
            } else {
                console.log("Respuesta de red OK pero respuesta HTTP no OK");
            }
        })
        .catch(function (err) {
            console.log(err, "response")
        });
}
export function GlobalAmount() {
    const publicBase = import.meta.env.VITE_API_URL || "https://localhost:7036";
    return axios({
        method: 'get',
        url: `${publicBase}/moneyboxes/getGlobalAmount`,
        headers: {
            'Content-Type': 'application/json',
        }
    })
        .then(function (response) {
            if (response.status === 200) {
                return Promise.resolve(response.data);
            } else {
                console.log("Respuesta de red OK pero respuesta HTTP no OK");
            }
        })
        .catch(function (err) {
            console.log(err, "response")
        });
}