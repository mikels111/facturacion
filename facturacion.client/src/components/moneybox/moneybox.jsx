import { useEffect, useState } from 'react';
import styles from './moneybox.module.css'
import { MoneyBoxesService } from '../../services/MoneyBoxService';
import Button from '../button/button';
export function Moneybox({ moneyBoxData, withdraw, singleDeposit }) {

    return (
        <div className={styles['square']}>
            <div className={styles['square-title']}>
                <h2>{moneyBoxData.name}</h2>
                <p>{moneyBoxData.description}</p>
            </div>
            <div className={styles['square-value']}>
                
                <p className={moneyBoxData.value <0 && styles['red-number'] }>{moneyBoxData.value}&euro;</p>
            </div>
            <div className={styles['square-buttons-wrap']}>
                <button className={`${styles['square-button']} ${styles['deposit']}`} onClick={() => { singleDeposit(moneyBoxData.id) }} >Ingresar</button>
                <button className={`${styles['square-button']} ${styles['withdraw']}`} onClick={() => { withdraw(moneyBoxData.id) }} >Retirar</button>

            </div>
        </div>
    );
}