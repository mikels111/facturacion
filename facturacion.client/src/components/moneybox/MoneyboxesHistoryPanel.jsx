import styles from './moneyboxes-history-panel.module.css';

function formatDate(value) {
    if (!value) return '';
    const typedDate = new Date(value);
    if (Number.isNaN(typedDate.getTime())) return String(value);
    return typedDate.toLocaleString();
}

function typeLabel(id) {
    if (id === 1) return '+';
    if (id === 2) return '-';
    return `${id ?? '-'}`;
}

export default function MoneyboxesHistoryPanel({ history }) {
    const items = Array.isArray(history) ? [...history] : [];

    items.sort((a, b) => {
        const ta = new Date(a?.date ?? 0).getTime();
        const tb = new Date(b?.date ?? 0).getTime();
        return tb - ta;
    });

    return (
        <aside className={styles.panel}>
            <h1 className={styles.title}>Histórico</h1>

            {items.length === 0 ? (
                <div className={styles.empty}>Sin movimientos</div>
            ) : (
                <ul className={styles.list}>
                    {items.map((item, idx) => {
                        const isWithdraw = item?.historyTypeId === 2;
                        const key = item?.date ?? idx;
                        return (
                            <li key={key} className={styles.item}>
                                <div className={styles.topRow}>
                                    <h2>{item?.moneybox}</h2>
                                    <h2 className={isWithdraw ? styles.amountRed : styles.amountBlue}>
                                        {typeLabel(item?.historyTypeId)} {item?.amount}€
                                    </h2>
                                </div>


                                <div className={styles.meta}>
                                    {item?.description && <h3 className={styles.desc}>{item?.description}</h3>}
                                    <h3>Fecha: {formatDate(item?.date)}</h3>
                                    {item?.moneyBoxPreviousAmount && <h3>Cantidad previa: {item?.moneyBoxPreviousAmount}€</h3>}
                                    <h3>Global previo: {item?.globalPreviousAmount}€</h3>
                                </div>
                            </li>
                        );
                    })}
                </ul>
            )}
        </aside>
    );
}
