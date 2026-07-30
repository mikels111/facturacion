import React, { useEffect, useState,useContext } from 'react';
import Modal from '@mui/material/Modal';
import Typography from '@mui/material/Typography';
import Button from '@mui/material/Button';
import Box from '@mui/material/Box';
import TextField from '@mui/material/TextField';
import styles from './moneybox.module.css'
import { MoneyBoxesService } from '../../services/MoneyBoxService';
import ButtonM from '../button/button';
import axios from 'axios';
import { Context } from '../../App';
import AddBoxIcon from '@mui/icons-material/AddBox';
import IndeterminateCheckBoxIcon from '@mui/icons-material/IndeterminateCheckBox';
import { pink, red,green} from '@mui/material/colors';

export function Moneybox({ moneyBoxData }) {
    // const [open, setOpen] = React.useState(false);
    const { getHistoryData, getGlobalAmount, getMoneyBoxes }= useContext(Context)
    const buttonStyle = {
        width: 95,
        fontSize: 10
    }
    const style = {
        position: 'absolute',
        top: '50%',
        left: '50%',
        transform: 'translate(-50%, -50%)',
        width: 500,
        bgcolor: 'background.paper',
        border: 'none',
        boxShadow: 24,
        borderRadius: 2,
        p: 4
    };
    const [open, setOpen] = React.useState(false);
    const handleOpen = () => setOpen(true);
    const handleClose = () => setOpen(false);
    const [formData, setFormData] = useState({
        amount: Number,
        description: ""
    });
    const [action, setAction] = useState();
    const [actualMoneyBox, setActualMoneybox] = useState();
    
    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({ ...prev, [name]: value }));
    };

    const singleDeposit = (e) => {
        //console.log("ingresar prueba");
        
        console.log(formData, "formData: " + e);
        let amount = formData.amount;
        if (amount != "" && amount > 0) {
            console.log("correcto")
            axios({
                method: 'post',
                url: `https://localhost:7036/moneyboxes/singledeposit/${e}`,
                headers: {
                    'Content-Type': 'application/json',
                },
                data: formData
            }).then(function (response) {
                if (response.status === 200) {
                    setFormData(false);
                    setOpen(false);
                    getMoneyBoxes();
                    getGlobalAmount();
                    getHistoryData();
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
       
        console.log(formData, "formData: " + e);
        if (formData.amount != "" && formData.amount > 0) {
            console.log("correcto")
            axios({
                method: 'post',
                url: `https://localhost:7036/moneyboxes/withdraw/${e}`,
                headers: {
                    'Content-Type': 'application/json',
                },
                data: formData
            }).then(function (response) {
                if (response.status === 200) {
                    setFormData(false);
                    setOpen(false);
                    getMoneyBoxes();
                    getGlobalAmount();
                    getHistoryData();
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
        <React.Fragment>


            <Modal
                open={open}
                onClose={handleClose}
                aria-labelledby="modal-modal-title"
                aria-describedby="modal-modal-description"
            >
                <Box sx={style}>
                    <div className="deposit-form">
                        {action == "singleDeposit" ? <h2>New Deposit <AddBoxIcon sx={{ color: green[500] }} /></h2> : <h2>New Withdraw <IndeterminateCheckBoxIcon sx={{ color: red[500] }} /></h2>} 
                        <h3>{moneyBoxData.name}</h3>
                        <TextField sx={{ width: 100 }} id="standard-basic" name="amount" label="Amount" variant="standard" type="number" value={formData.amount} autoFocus="true" onChange={handleChange} />
                        <TextField id="standard-basic" name="description" label="Description" variant="standard"  value={formData.description} onChange={handleChange} />
                        

                        <div className="deposit-form-buttons">
                            <Button sx={buttonStyle} variant="contained" onClick={(e) => {
                                e.preventDefault();
                                e.stopPropagation();
                                if (action === "singleDeposit") {
                                    singleDeposit(actualMoneyBox);
                                } else {
                                    withdraw(actualMoneyBox);
                                }
                            }}>
                                Aceptar
                            </Button>
                            <Button sx={buttonStyle} variant="outlined" onClick={(e) => {
                                e.preventDefault();
                                e.stopPropagation();
                                handleClose();
                            }}>
                                Cancelar
                            </Button>
                        </div>
                    </div>

                </Box>
            </Modal>
            <div className={styles['square']}>
                <div className={styles['square-title']}>
                    <h2>{moneyBoxData.name}</h2>
                    <p>{moneyBoxData.description}</p>
                </div>
                <div className={styles['square-value']}>

                    <p className={moneyBoxData.value < 0 && styles['red-number']}>{moneyBoxData.valueString}&euro;</p>
                </div>
                <div className={styles['square-buttons-wrap']}>
                    <Button sx={buttonStyle} variant="contained" onClick={(e) => {
                        e.preventDefault();
                        e.stopPropagation();
                        // onClickEvent();
                        handleOpen();
                        setAction("singleDeposit");
                        setActualMoneybox(moneyBoxData.id);
                    }} >
                        &nbsp;Ingresar&nbsp;&nbsp;<AddBoxIcon />
                    </Button>
                    <Button sx={buttonStyle} variant="outlined" onClick={(e) => {
                        e.preventDefault();
                        e.stopPropagation();
                        handleOpen();
                        setAction("withdraw");
                        setActualMoneybox(moneyBoxData.id);
                    }} >
                        &nbsp;&nbsp;Retirar&nbsp;&nbsp;<IndeterminateCheckBoxIcon />
                    </Button>


                </div>
            </div>
        </React.Fragment>
    );
}
