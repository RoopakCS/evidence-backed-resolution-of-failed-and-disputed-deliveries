
import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import "./Register.css";
import {
    registerUser
} from '../services/authService'

import {
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

        // Validate form
        const validationError = validateForm();

        if (validationError) {
            setError(validationError);
            return;
        }

        setLoading(true);

        try {
            const result = await registerUser(formData);

            if (result.success) {
                setSuccess(
                    result.message || "Registration successful!"
                );

                // Clear form
                setFormData({
                    firstname: "",
                    lastname: "",
                    email: "",
                    phonenumber: "",
                    password: "",
                    confirmpassword: "",
                });

                // Navigate to login page
                setTimeout(() => {
                    navigate("/login");
                }, 1200);

            } else {
                setError(
                    result.message || "Registration failed."
                );
            }

        } catch (err) {
            console.error("Registration error:", err);

            setError(
                "Something went wrong. Please try again."
            );

        } finally {
            setLoading(false);
        }
    };

    return (
        <main className="register-page-container">
            <section className="register-card-wrapper">

                {/* Header */}
                <div className="register-header-group">
                    <h1 className="register-headline">
                        Create your account
                    </h1>

                    <p className="register-subheadline">
                        Get started by creating your Docket account.
                    </p>
                </div>

                {/* Form */}
                <form
                    onSubmit={handleSubmit}
                    className="register-form"
                >

                    <div className="register-form-grid">

                        {/* First Name */}
                        <div className="register-field-group">
                            <label htmlFor="firstname">
                                First Name
                            </label>

                            <div className="register-input-container">
                                <UserRound size={18} className="register-input-icon" />

                                <input
                                    id="firstname"
                                    type="text"
                                    name="firstname"
                                    placeholder="First name"
                                    value={formData.firstname}
                                    onChange={handleChange}
                                    maxLength={50}
                                    autoComplete="given-name"
                                />
                            </div>
                        </div>

                        {/* Last Name */}
                        <div className="register-field-group">
                            <label htmlFor="lastname">
                                Last Name
                            </label>

                            <div className="register-input-container">
                                <UserRound size={18} className="register-input-icon" />

                                <input
                                    id="lastname"
                                    type="text"
                                    name="lastname"
                                    placeholder="Last name"
                                    value={formData.lastname}
                                    onChange={handleChange}
                                    maxLength={50}
                                    autoComplete="family-name"
                                />
                            </div>
                        </div>

                        {/* Email */}
                        <div className="register-field-group full-width">
                            <label htmlFor="email">
                                Email Address
                            </label>

                            <div className="register-input-container">
                                <Mail size={18} className="register-input-icon" />

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

                        {/* Phone */}
                        <div className="register-field-group full-width">
                            <label htmlFor="phonenumber">
                                Phone Number
                            </label>

                            <div className="register-input-container">
                                <Phone size={18} className="register-input-icon" />

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

                        {/* Password */}
                        <div className="register-field-group full-width">
                            <label htmlFor="password">
                                Password
                            </label>

                            <div className="register-input-container">
                                <LockKeyhole size={18} className="register-input-icon" />

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
                                    className="register-password-toggle"
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

                            <span className="register-input-hint">
                                Minimum 8 characters with uppercase,
                                lowercase, number and special character.
                            </span>
                        </div>

                        {/* Confirm Password */}
                        <div className="register-field-group full-width">
                            <label htmlFor="confirmpassword">
                                Confirm Password
                            </label>

                            <div className="register-input-container">
                                <LockKeyhole size={18} className="register-input-icon" />

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
                                    className="register-password-toggle"
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

                    {/* Error Message */}
                    {error && (
                        <div className="register-feedback error">
                            <AlertCircle size={18} />
                            <span>{error}</span>
                        </div>
                    )}

                    {/* Success Message */}
                    {success && (
                        <div className="register-feedback success">
                            <CheckCircle size={18} />
                            <span>{success}</span>
                        </div>
                    )}

                    {/* Submit Button */}
                    <button
                        type="submit"
                        className="register-submit-button"
                        disabled={loading}
                    >
                        {loading ? (
                            <>
                                <LoaderCircle className="register-spinner" size={18} />
                                Creating Account...
                            </>
                        ) : (
                            "Create Account"
                        )}
                    </button>

                </form>

                {/* Footer */}
                <div className="register-footer-section">
                    <p>
                        Already have an account?
                        <Link to="/login" className="register-login-link">
                            Sign in
                        </Link>
                    </p>
                </div>

            </section>
        </main>
    );
}

export default Register;
