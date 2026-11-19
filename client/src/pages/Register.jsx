
import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import axios from "axios";

import {
    ShieldCheck,
    UserRound,
    Mail,
    Phone,
    LockKeyhole,
    Eye,
    EyeOff,
    LoaderCircle,
    CheckCircle,
    AlertCircle,
} from "lucide-react";

function Register() {
    const navigate = useNavigate();

    const [formData, setFormData] = useState({
        firstname: "",
        lastname: "",
        email: "",
        phonenumber: "",
        password: "",
        confirmpassword: "",
    });

    const [showPassword, setShowPassword] = useState(false);
    const [showConfirmPassword, setShowConfirmPassword] = useState(false);

    const [loading, setLoading] = useState(false);
    const [error, setError] = useState("");
    const [success, setSuccess] = useState("");

    const handleChange = (e) => {
        const { name, value } = e.target;

        setFormData((prev) => ({
            ...prev,
            [name]: value,
        }));
    };

    const validateForm = () => {
        const {
            firstname,
            lastname,
            email,
            phonenumber,
            password,
            confirmpassword,
        } = formData;

        if (
            !firstname.trim() ||
            !lastname.trim() ||
            !email.trim() ||
            !phonenumber.trim() ||
            !password ||
            !confirmpassword
        ) {
            return "Please fill in all the fields.";
        }

        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

        if (!emailRegex.test(email)) {
            return "Please enter a valid email address.";
        }

        const phoneRegex = /^\+?[0-9\s()-]{10,15}$/;

        if (!phoneRegex.test(phonenumber)) {
            return "Please enter a valid phone number.";
        }

        const passwordRegex =
            /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/;

        if (!passwordRegex.test(password)) {
            return "Password must contain at least 8 characters, uppercase, lowercase, number and special character.";
        }

        if (password !== confirmpassword) {
            return "Passwords do not match.";
        }

        return null;
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        setError("");
        setSuccess("");

        const validationError = validateForm();

        if (validationError) {
            setError(validationError);
            return;
        }

        setLoading(true);

        try {
            const response = await axios.post(
                `${import.meta.env.VITE_API_BASE_URL}/Auth/register`,
                formData
            );

            if (response.data.success) {
                setSuccess(
                    response.data.message || "Registration successful!"
                );

                setFormData({
                    firstname: "",
                    lastname: "",
                    email: "",
                    phonenumber: "",
                    password: "",
                    confirmpassword: "",
                });

                setTimeout(() => {
                    navigate("/login");
                }, 1200);
            } else {
                setError(
                    response.data.message || "Registration failed."
                );
            }
        } catch (err) {
            const data = err.response?.data;

            const validationMessages = data?.errors
                ? Object.values(data.errors).flat().join(" ")
                : "";

            const serverMessage =
                data?.message ||
                data?.Message ||
                validationMessages ||
                (typeof data === "string" ? data : "");

            setError(
                serverMessage || "Something went wrong. Please try again."
            );
        } finally {
            setLoading(false);
        }
    };

    return (
        <main className="register-page">
            <section className="register-card">

                <div className="brand">
                    <div className="brand-icon">
                        <ShieldCheck size={25} />
                    </div>
                    <span>DOCKET</span>
                </div>

                <div className="register-heading">
                    <h1>Create your account</h1>
                   
                </div>

                <form onSubmit={handleSubmit} className="register-form">

                    <div className="form-grid">

                        
                        <div className="form-group">
                            <label htmlFor="firstname">First Name</label>

                            <div className="input-wrapper">
                                <UserRound className="input-icon" size={18} />

                                <input
                                    id="firstname"
                                    type="text"
                                    name="firstname"
                                    placeholder="Enter first name"
                                    value={formData.firstname}
                                    onChange={handleChange}
                                    maxLength={50}
                                    autoComplete="given-name"
                                />
                            </div>
                        </div>

                        <div className="form-group">
                            <label htmlFor="lastname">Last Name</label>

                            <div className="input-wrapper">
                                <UserRound className="input-icon" size={18} />

                                <input
                                    id="lastname"
                                    type="text"
                                    name="lastname"
                                    placeholder="Enter last name"
                                    value={formData.lastname}
                                    onChange={handleChange}
                                    maxLength={50}
                                    autoComplete="family-name"
                                />
                            </div>
                        </div>

                        <div className="form-group full-width">
                            <label htmlFor="email">Email Address</label>

                            <div className="input-wrapper">
                                <Mail className="input-icon" size={18} />

                                <input
                                    id="email"
                                    type="email"
                                    name="email"
                                    placeholder="Enter your email address"
                                    value={formData.email}
                                    onChange={handleChange}
                                    maxLength={100}
                                    autoComplete="email"
                                />
                            </div>
                        </div>

                        <div className="form-group full-width">
                            <label htmlFor="phonenumber">Phone Number</label>

                            <div className="input-wrapper">
                                <Phone className="input-icon" size={18} />

                                <input
                                    id="phonenumber"
                                    type="tel"
                                    name="phonenumber"
                                    placeholder="Enter your phone number"
                                    value={formData.phonenumber}
                                    onChange={handleChange}
                                    maxLength={15}
                                    autoComplete="tel"
                                />
                            </div>
                        </div>

                        <div className="form-group full-width">
                            <label htmlFor="password">Password</label>

                            <div className="input-wrapper">
                                <LockKeyhole className="input-icon" size={18} />

                                <input
                                    id="password"
                                    type={showPassword ? "text" : "password"}
                                    name="password"
                                    placeholder="Create a password"
                                    value={formData.password}
                                    onChange={handleChange}
                                    autoComplete="new-password"
                                />

                                <button
                                    type="button"
                                    className="password-toggle"
                                    onClick={() => setShowPassword(!showPassword)}
                                    aria-label="Toggle password visibility"
                                >
                                    {showPassword ? (
                                        <EyeOff size={18} />
                                    ) : (
                                        <Eye size={18} />
                                    )}
                                </button>
                            </div>

                            <span className="input-hint">
                                Minimum 8 characters with uppercase, lowercase,
                                number and special character.
                            </span>
                        </div>

                        <div className="form-group full-width">
                            <label htmlFor="confirmpassword">
                                Confirm Password
                            </label>

                            <div className="input-wrapper">
                                <LockKeyhole className="input-icon" size={18} />

                                <input
                                    id="confirmpassword"
                                    type={showConfirmPassword ? "text" : "password"}
                                    name="confirmpassword"
                                    placeholder="Confirm your password"
                                    value={formData.confirmpassword}
                                    onChange={handleChange}
                                    autoComplete="new-password"
                                />

                                <button
                                    type="button"
                                    className="password-toggle"
                                    onClick={() =>
                                        setShowConfirmPassword(!showConfirmPassword)
                                    }
                                    aria-label="Toggle confirm password visibility"
                                >
                                    {showConfirmPassword ? (
                                        <EyeOff size={18} />
                                    ) : (
                                        <Eye size={18} />
                                    )}
                                </button>
                            </div>
                        </div>

                    </div>

                    {error && (
                        <div className="message error-message">
                            <AlertCircle size={18} />
                            <span>{error}</span>
                        </div>
                    )}

                    {success && (
                        <div className="message success-message">
                            <CheckCircle size={18} />
                            <span>{success}</span>
                        </div>
                    )}

                    <button
                        type="submit"
                        className="register-button"
                        disabled={loading}
                    >
                        {loading ? (
                            <>
                                <LoaderCircle className="spinner" size={19} />
                                Creating Account...
                            </>
                        ) : (
                            "Create Account"
                        )}
                    </button>

                </form>

                <div className="login-link">
                    Already have an account?
                    <Link to="/login">Sign in</Link>
                </div>

                

            </section>
        </main>
    );
}

export default Register;
