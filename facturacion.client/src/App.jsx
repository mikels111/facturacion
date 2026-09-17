import React, { StrictMode, useEffect, useState } from 'react';
import './App.css';
import { Moneybox } from './components/moneybox/moneybox';
import MoneyboxesHistoryPanel from './components/moneybox/MoneyboxesHistoryPanel';
import { MoneyBoxesService, GlobalAmount, GetHistory, GetHistoryForEachBoxLastMonth, GetHistoryGlobalAmountOneMonth } from '../src/services/MoneyBoxService';
import axios from 'axios';
import ButtonM from './components/button/button';
import Modal from '@mui/material/Modal';
import Typography from '@mui/material/Typography';
import Button from '@mui/material/Button';
import Box from '@mui/material/Box';
import TextField from '@mui/material/TextField';
export const Context = React.createContext();
import AddBoxIcon from '@mui/icons-material/AddBox';
import IndeterminateCheckBoxIcon from '@mui/icons-material/IndeterminateCheckBox';
import Divider from '@mui/material/Divider';
import Tabs from '@mui/material/Tabs';
import Tab from '@mui/material/Tab';
import { RechartsDevtools } from '@recharts/devtools';
import { BarChart, Bar, Area, AreaChart, XAxis, YAxis, CartesianGrid, Tooltip, Legend } from 'recharts';
function App() {
    const [moneyBoxes, setMoneyBoxes] = useState();
    const [globalAmountValue, setGlobalAmountValue] = useState();
    const [history, setHistory] = useState([]);
    const [formData, setFormData] = useState({
        amount: Number,
        description: ""
    });
    const [history4weeks, setHistory4weeks] = useState();
    const [historyAmnt4weeks, setHistoryAmnt4weeks] = useState();
    const [tabValue, setTabValue] = useState(0);

    const getHistoryData = () => {
        GetHistory()
            .then((response) => {
                setHistory(response);
            })
            .catch((err) => {
                console.log("App error-> ", err);
            });
    }
    const getMoneyBoxes = () => {
        MoneyBoxesService()
            .then((response) => {
                setMoneyBoxes(response);
            })
            .catch((err) => {
                console.log("App error-> ", err);
            });
    }
    const getGlobalAmount = () => {
        GlobalAmount()
            .then((response) => {
                console.log("getglobalamount: ", response)
                setGlobalAmountValue(response);
            })
            .catch((err) => {
                console.log("App error-> ", err);
            });
    }
    const getHistoryForEachBoxLastMonth = () => {
        GetHistoryForEachBoxLastMonth()
            .then((response) => {
                console.log("gethistoryforeachboxlastmonth: ", response)
                setHistory4weeks(response);
            })
            .catch((err) => {
                console.log("App error-> ", err);
            });
    }
    const getHistoryGlobalAmountOneMonth = () => {
        GetHistoryGlobalAmountOneMonth()
            .then((response) => {
                console.log("gethistoryforeachboxlastmonth: ", response)
                setHistoryAmnt4weeks(response);
            })
            .catch((err) => {
                console.log("App error-> ", err);
            });
    }

    useEffect(() => {

        getMoneyBoxes();
        getGlobalAmount();
        getHistoryData();
        getHistoryForEachBoxLastMonth();
        getHistoryGlobalAmountOneMonth();


    }, []);

    useEffect(() => {
        console.log("history: ", history4weeks);
    }, [history4weeks]);



    const deposit = () => {
        // e.preventDefault();
        // e.stopPropagation();
        console.log(formData, "formData: ");
        console.log("ingresar prueba");
        // console.log(e);
        if (formData.amount != "" && formData.amount > 0) {
            console.log("correcto")
            axios({
                method: 'post',
                url: `https://localhost:7036/moneyboxes/deposit`,
                headers: {
                    'Content-Type': 'application/json',
                },
                data: formData
            }).then(function (response) {
                if (response.status === 200) {
                    handleClose();
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


    const [open, setOpen] = React.useState(false);
    const handleOpen = () => setOpen(true);
    const handleClose = () => setOpen(false);
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
    const buttonStyle = {
        width: 95,
        fontSize: 10
    }

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({ ...prev, [name]: value }));
    };
    const handleChangeTab = (event, newValue) => {
        setTabValue(newValue);
    };
    return (
        <Context.Provider value={{
            getMoneyBoxes,
            getGlobalAmount,
            getHistoryData
        }}>


            <React.Fragment>
                <div className="app-root">
                    <Modal
                        open={open}
                        onClose={handleClose}
                        aria-labelledby="modal-modal-title"
                        aria-describedby="modal-modal-description"
                    >
                        <Box sx={style}>
                            <div className="deposit-form">
                                <h2>New global deposit <AddBoxIcon /></h2>
                                <TextField sx={{ width: 100 }} id="standard-basic" name="amount" label="Amount" variant="standard" type="number" value={formData.amount} autoFocus="true" onChange={handleChange} />
                                <TextField id="standard-basic" name="description" label="Description" variant="standard" value={formData.description} onChange={handleChange} />


                                <div className="deposit-form-buttons">
                                    <Button sx={buttonStyle} variant="contained" onClick={() => { deposit(); }} >Aceptar</Button>
                                    <Button sx={buttonStyle} variant="outlined" onClick={handleClose} >Cancelar</Button>
                                </div>
                            </div>

                        </Box>
                    </Modal>
                    <div className="header">
                        <div>
                            <h1>{globalAmountValue}&euro;</h1>
                            <h3>Global Amount</h3>
                        </div>

                        <Button sx={buttonStyle} variant="contained" onClick={handleOpen} >&nbsp;Ingresar&nbsp;&nbsp;<AddBoxIcon /></Button>
                    </div>

                    <div className="main-layout">
                        <div className="main-1">
                            <div className="squares-container">
                                {moneyBoxes &&
                                    moneyBoxes.map((moneyBox, i) => {
                                        return (
                                            <>
                                                <Moneybox key={i} moneyBoxData={moneyBox} />
                                                <Divider orientation="vertical" flexItem />
                                            </>
                                        )
                                    })
                                }



                            </div>
                            


                            <Box sx={{ width: '100%' }}>
                                <Tabs
                                    value={tabValue}
                                    onChange={handleChangeTab}
                                    aria-label="wrapped label tabs example"
                                >
                                    <Tab label="Amount" {...a11yProps(0)} />
                                    <Tab label="Expenses" {...a11yProps(1)} />
                                </Tabs>
                                <CustomTabPanel value={tabValue} index={0}>
                                    <h2>last four weeks</h2>
                                    <AreaChart
                                        style={{ width: '100%', maxWidth: '10000px', maxHeight: '50vh', aspectRatio: 1.618 }}
                                        responsive
                                        data={historyAmnt4weeks}
                                        margin={{ top: 10, right: 0, left: 0, bottom: 0 }}
                                        value={tabValue}
                                    >
                                        <defs>
                                            <linearGradient id="colorUv" x1="0" y1="0" x2="0" y2="1">
                                                <stop offset="5%" stopColor="#8884d8" stopOpacity={0.8} />
                                                <stop offset="95%" stopColor="#8884d8" stopOpacity={0} />
                                            </linearGradient>
                                        </defs>
                                        <CartesianGrid strokeDasharray="3 3" />
                                        <XAxis dataKey="date" />
                                        <YAxis width="1px" />
                                        <Tooltip />
                                        <Area
                                            type="monotone"
                                            dataKey="amount"
                                            stroke="#1976D2"
                                            fillOpacity={1}
                                            fill="url(#colorPv)"
                                            isAnimationActive={true}
                                        />
                                        <RechartsDevtools />

                                    </AreaChart>
                                </CustomTabPanel>
                                <CustomTabPanel value={tabValue} index={1}>
                                    <h2>last four weeks</h2>
                                    <BarChart
                                        style={{ width: '100%', maxWidth: '1000px', maxHeight: '50vh', aspectRatio: 1.618 }}
                                        responsive
                                        data={history4weeks}
                                        margin={{
                                            top: 5,
                                            right: 0,
                                            left: 0,
                                            bottom: 5,
                                        }}
                                    >
                                        <CartesianGrid strokeDasharray="3 3" />
                                        <XAxis dataKey="name" />
                                        <YAxis width="1px" />
                                        <Tooltip />
                                        <Legend />
                                        <Bar dataKey="inversion" fill="#fcba03" activeBar={{ fill: 'pink', stroke: 'blue' }} />
                                        <Bar dataKey="gastosBasicos" fill="#ca80ff" activeBar={{ fill: 'gold', stroke: 'purple' }} />
                                        <Bar dataKey="ocio" fill="#5784ff" activeBar={{ fill: 'gold', stroke: 'purple' }} />
                                        <Bar dataKey="gastosGrandes" fill="#0af7ff" activeBar={{ fill: 'gold', stroke: 'purple' }} />
                                        <Bar dataKey="donacion" fill="#82ca9d" activeBar={{ fill: 'gold', stroke: 'purple' }} />


                                        <RechartsDevtools />
                                    </BarChart>
                                </CustomTabPanel>

                            </Box>
                        </div>
                        <MoneyboxesHistoryPanel history={history} />

                    </div>
                </div>
            </React.Fragment>
        </Context.Provider>
    );


}
function a11yProps(index) {
    return {
        id: `simple-tab-${index}`,
        'aria-controls': `simple-tabpanel-${index}`,
    };
}
function CustomTabPanel(props) {
    const { children, value, index, ...other } = props;

    return (
        <div
            role="tabpanel"
            hidden={value !== index}
            id={`simple-tabpanel-${index}`}
            aria-labelledby={`simple-tab-${index}`}
            {...other}
        >
            {value === index && <Box sx={{ p: 3 }}>{children}</Box>}
        </div>
    );
}

export default App;
