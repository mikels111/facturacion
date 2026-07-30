import React, { StrictMode, useEffect } from 'react';
import Modal from 'react-bootstrap/Modal';
import Form from 'react-bootstrap/Form';
import Button from 'react-bootstrap/Button';
function ModalForm(props) {
    const action = (props, e) => {
        e.preventDefault();
        console.log(e);
    }
    useEffect(() => {
        console.log("modal");
    }, [])
    return (
        <Modal
            show={props.show}
            onHide={props.onHide}
            aria-labelledby="contained-modal-title-vcenter"
            animation={false}
            centered
        >
            <Modal.Header closeButton>
                <Modal.Title id="contained-modal-title-vcenter">
                    New Project
                </Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Form noValidate onSubmit={(e) => { action(props, e); }}>
                    <Form.Group className="mb-3" controlId="formBasicEmail" >
                        <Form.Label>Project Name</Form.Label>
                    </Form.Group>
                    <Button variant="flat" type="submit">
                        Create
                    </Button>
                </Form>
            </Modal.Body>
        </Modal>
    );
}

export default Modal;